using IniParser.Model;
using SALT.Config.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SALT.Config
{
    public class ConfigFile
    {
        private const string INI = "ini";

        /// <summary>
        /// Name of <see cref="ConfigFile"/>
        /// </summary>
        public string FileName { get; internal set; }

        /// <summary>
        /// Default section of <see cref="ConfigFile"/>
        /// </summary>
        public string DefaultSection { get; internal set; }

        readonly Dictionary<string, ConfigSection> sections = new Dictionary<string, ConfigSection>();

        /// <summary>
        /// All sections of the <see cref="ConfigFile"/>
        /// </summary>
        public IEnumerable<ConfigSection> Sections => sections.Values;

        /// <summary>
        /// Adds a <see cref="ConfigSection"/> to the <see cref="ConfigFile"/>
        /// </summary>
        /// <param name="section">Section to add</param>
        public void AddSection(ConfigSection section)
        {
            sections[section.Name] = section;
        }

        /// <summary>
        /// Creates a new <see cref="ConfigSection"/> and adds it to the <see cref="ConfigFile"/>
        /// </summary>
        /// <param name="name">Name of <see cref="ConfigSection"/></param>
        /// <returns>The created <see cref="ConfigSection"/></returns>
        public ConfigSection AddSection(string name)
        {
            var section = new ConfigSection(name);
            AddSection(section);
            return section;
        }

        /// <summary>
        /// Gets a <see cref="ConfigSection"/> from the <see cref="ConfigFile"/>
        /// </summary>
        /// <param name="index">Name of the <see cref="ConfigSection"/></param>
        /// <returns>A <see cref="ConfigSection"/> for the provided index</returns>
        public ConfigSection this[string index] => sections[index];

        /// <summary>
        /// Loads the Ini Data
        /// </summary>
        /// <param name="data">The ini data to load</param>
        public void LoadIniData(IniData data)
        {

            foreach (var section in data.Sections)
            {
                if (sections.TryGetValue(section.SectionName, out var confsect))
                {
                    confsect.LoadIniData(section.Keys);
                }
                else
                {
                    Debug.LogWarning($"Unknown section {section.SectionName} in data! Ignoring...");
                }
            }
        }

        /// <summary>
        /// Writes the Ini Data
        /// </summary>
        /// <param name="data">The ini data to write</param>
        public void WriteIniData(IniData data)
        {
            foreach (var sectiondata in sections)
            {
                var section = new SectionData(sectiondata.Key);
                sectiondata.Value.WriteIniData(section.Keys);
                if (sectiondata.Value.Comment != null) section.Comments.Add(sectiondata.Value.Comment);
                data.Sections.Add(section);
            }
        }

        /// <summary>
        /// Generate a <see cref="ConfigFile"/> for the provided <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type to generate a config for.</param>
        /// <returns>The generated <see cref="ConfigFile"/></returns>
        public static ConfigFile GenerateConfig(Type type)
        {
            var file = new ConfigFile();
            var attribute = type.GetCustomAttributes(typeof(ConfigFileAttribute), false).FirstOrDefault() as ConfigFileAttribute;
            if (attribute == null) return null;
            file.FileName = attribute.FileName;
            var defaultSection = file.AddSection(attribute.DefaultSection);
            foreach (var field in type.GetFields().Where(x => x.IsStatic))
            {
                defaultSection.AddElement(new FieldBackedConfigElement(field));
            }
            foreach (var v in type.GetNestedTypes())
            {
                var sectionAttribute = v.GetCustomAttributes(typeof(ConfigSectionAttribute), false).FirstOrDefault() as ConfigSectionAttribute;
                if (sectionAttribute == null) continue;
                var newSection = file.AddSection(sectionAttribute.SectionName ?? v.Name);
                foreach (var field in v.GetFields().Where(x => x.IsStatic))
                {
                    newSection.AddElement(new FieldBackedConfigElement(field));
                }
            }
            return file;
        }

        /// <summary>
        /// Tries to load the <see cref="ConfigFile"/>
        /// </summary>
        /// <returns><see langword="true"/> if file was loaded, <see langword="false"/> if it was not.</returns>
        public bool TryLoadFromFile()//bool writeInDefault = true)
        {
            var configpath = FileSystem.GetMyConfigPath();
            var parser = new IniParser.FileIniDataParser();
            var filePath = Path.ChangeExtension(Path.Combine(configpath, FileName), INI);
            IniData data;
            try
            {
                data = parser.ReadFile(filePath);
            }
            catch
            {
                SaveToFile();
                return false;
            }
            LoadIniData(data);
            SaveToFile();
            return true;

        }

        /// <summary>
        /// Saves the <see cref="ConfigFile"/>
        /// </summary>
        public void SaveToFile()
        {
            var parser = new IniParser.FileIniDataParser();
            var filePath = Path.ChangeExtension(Path.Combine(FileSystem.GetMyConfigPath(), FileName), INI);
            var data = new IniData();
            WriteIniData(data);
            parser.WriteFile(filePath, data);
        }

    }
}