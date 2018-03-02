using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ExceptionLogParser
{
    public class CategoriesConfigurationHandler
    {
        public static CategoriesConfigsSection _Config = ConfigurationManager.GetSection("categoriesConfigs") as CategoriesConfigsSection;

        public static CategoriesElementCollection GetCategories()
        {
            return _Config.Categories;
        }
    }
    
    public class CategoriesConfigsSection : ConfigurationSection
    {
        [ConfigurationProperty("categories")]
        public CategoriesElementCollection Categories
        {
            get { return (CategoriesElementCollection)this["categories"]; }
        }
    }
    
    [ConfigurationCollection(typeof(CategoriesElement))]
    public class CategoriesElementCollection : ConfigurationElementCollection
    {
        public CategoriesElement this[int index]
        {
            get { return (CategoriesElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CategoriesElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CategoriesElement)element).RawText;
        }
    }

    public class CategoriesElement : ConfigurationElement
    {
        [ConfigurationProperty("rawtext", IsRequired = true)]
        public string RawText
        {
            get { return (string)this["rawtext"]; }
            set { this["rawtext"] = value; }
        }

        [ConfigurationProperty("plaintext", IsRequired = true)]
        public string PlainText
        {
            get { return (string)this["plaintext"]; }
            set { this["plaintext"] = value; }
        }
    }
}
