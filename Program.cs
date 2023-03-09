using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Datenauswertungstool
{
    class Program
    {
        static void Main(string[] args)
        {
            // Laden der input.xml- und action.xml-Dateien
            XDocument inputXml = XDocument.Load(@"C:\Users\x\Desktop\Icsys-bewerber-aufgabe\DataToolAufgabe\DataToolAufgabe\input.xml");//Path ämdern
            XDocument actionXml = XDocument.Load(@"C:\Users\x\Desktop\Icsys-bewerber-aufgabe\DataToolAufgabe\DataToolAufgabe\action.xml");//Path ämdern

            // Erstellung einer Liste von company-Elementen
            List<XElement> company = inputXml.Descendants("company").ToList();

            // Erstellung eines Dictionarys zur Speicherung der Ergebnisse
            Dictionary<string, string> results = new Dictionary<string, string>();

            // Schleife durch jedes action-Element in action.xml und Ausführung der angeforderten Aktion
            foreach (XElement action in actionXml.Descendants("action"))
            {
                string name = action.Attribute("name").Value;
                string type = action.Attribute("type").Value;
                string function = action.Attribute("function").Value;
                string source = action.Attribute("source").Value;
                string filter = action.Attribute("filter").Value;

                // Filtern der Liste von company basierend auf dem Filter-Regex
                List<XElement> filteredCompany = company.Where(c => System.Text.RegularExpressions.Regex.IsMatch(c.Attribute("name").Value, filter)).ToList();

                // Ausführung der angeforderten Action auf der gefilterten Company
                string result;
                if (type == "element")
                {
                    if (function == "sum")
                    {
                        result = filteredCompany.Sum(c => decimal.Parse(c.Element(source).Value)).ToString();
                    }
                    else if (function == "max")
                    {
                        result = filteredCompany.Max(c => decimal.Parse(c.Element(source).Value)).ToString();
                    }
                    else if (function == "min")
                    {
                        result = filteredCompany.Min(c => decimal.Parse(c.Element(source).Value)).ToString();
                    }
                    else if (function == "average")
                    {
                        result = filteredCompany.Average(c => decimal.Parse(c.Element(source).Value)).ToString();
                    }
                    else
                    {
                        throw new InvalidOperationException("Ungültige Funktion für Element-Aktion angegeben");
                    }
                }
                else if (type == "attribute")
                {
                    if (function == "sum")
                    {
                        result = filteredCompany.Sum(c => int.Parse(c.Attribute(source).Value)).ToString();
                    }
                    else
                    {
                        throw new InvalidOperationException("Ungültige Funktion für Attribut-Aktion angegeben");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Ungültiger Typ für Aktion angegeben");
                }

                // Hinzufügen des Ergebnisses zum results-Dictionary
                results.Add(name, result);
            }

            // Erstellung der result.xml-Datei
            XDocument resultXml = new XDocument(
                new XElement("results",
                    results.Select(r => new XElement("result", new XAttribute("name", r.Key), 
                    decimal.Parse(r.Value).ToString("0.00", CultureInfo.GetCultureInfo("de-DE"))))
                )
            );
            resultXml.Save(@"C:\Users\x\Desktop\Icsys-bewerber-aufgabe\DataToolAufgabe\DataToolAufgabe\result.xml");
        }
    }
}
