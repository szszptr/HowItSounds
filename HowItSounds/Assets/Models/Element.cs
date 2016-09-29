using System.Collections.Generic;
using System.Xml;

namespace HowItSounds.Assets.Models
{
    class Element
    {
        //Class for containing properties.
        public string Name { get; set; }
        public string CoverImage { get; set; }
        public string Sound { get; set; }
        public int Category { get; set; }

        public Element(string _name, string _image, string _sound, int _category)
        {
            Name = _name;
            CoverImage = _image;
            Sound = _sound;
            Category = _category;
        }
    }
    class ElementFactory
    {
        //Factory
        public static List<Element> GetElements(string uriString, string filename)
        {
            string _uri = uriString; //Get Config file and elements location
            string _filename = filename; // Get Config.xml filename
            string _connect = _uri + filename; // Create connection string
            List<Element> _elements = new List<Element>(); // New list for the elements
            try
            {
                using (XmlReader reader = XmlReader.Create(_connect)) // Read the Config.xml
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            string _begin = @"ms-appx:///"; // or could try this.BaseUri
                            reader.ReadToFollowing("name");
                            string name = reader.ReadElementContentAsString(); // Car
                            reader.ReadToFollowing("image");
                            string image = _begin + _uri + reader.ReadElementContentAsString(); // ms-appx:///Assets/Elements/car.jpg
                            reader.ReadToFollowing("sound");
                            string sound = _begin + _uri + reader.ReadElementContentAsString(); // ms-appx:///Assets/Elements/car.wav
                            reader.ReadToFollowing("category");
                            int category = reader.ReadElementContentAsInt(); // 1
                            _elements.Add(new Element(name, image, sound, category));
                        }
                    }
                }
            }
            catch (System.IO.FileNotFoundException) { return _elements; }
            return _elements;
        }
    }
}
