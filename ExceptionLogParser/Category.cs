using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;

namespace ExceptionLogParser
{
    class Category
    {
        public string rawText;
        public string plainText;
        
        public Category() {
            rawText = "";
            plainText = "";
        }

        public void SetText(string rawText, string plainText)
        {
            this.rawText = rawText;
            this.plainText = plainText;
        }

        public static List<Category> GetAvailableCategories()
        {
            List<Category> categoriesReturn = new List<Category>();
            foreach( CategoriesElement category in CategoriesConfigurationHandler.GetCategories())
            {
                Category tempCategory = new Category();
                tempCategory.SetText(category.RawText, category.PlainText);
                categoriesReturn.Add(tempCategory);
            }

            return categoriesReturn;
        }

        public static void AddCategoryToConfigFile(string rawText, string plainText)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            // create new node <add key="Region" value="Canterbury" />
            var nodeCategory = xmlDoc.CreateElement("add");
            nodeCategory.SetAttribute("plaintext", plainText);
            nodeCategory.SetAttribute("rawtext", rawText);

            xmlDoc.SelectSingleNode("//categoriesConfigs/categories").AppendChild(nodeCategory);
            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            ConfigurationManager.RefreshSection("categoriesConfigs/categories");
        }
    }
}
