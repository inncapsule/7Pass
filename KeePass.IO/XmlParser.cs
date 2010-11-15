﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace KeePass.IO
{
    internal class XmlParser
    {
        public Group Parse(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var settings = new XmlReaderSettings
            {
                CloseInput = false,
                IgnoreComments = true,
                IgnoreWhitespace = true,
                IgnoreProcessingInstructions = true,
            };

            using (var reader = XmlReader.Create(stream, settings))
            {
                if (!reader.ReadToFollowing("KeePassFile"))
                    return null;

                if (!reader.ReadToDescendant("Root"))
                    return null;

                if (!reader.ReadToDescendant("Group"))
                    return null;

                using (var subReader = reader.ReadSubtree())
                    return ParseGroup(subReader);
            }
        }

        private static void ParseChildren(XmlReader reader, Group group)
        {
            while (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "Group":
                        using (var subReader = reader.ReadSubtree())
                            group.Add(ParseGroup(subReader));
                        reader.ReadEndElement();
                        break;

                    case "Entry":
                        using (var subReader = reader.ReadSubtree())
                            group.Add(ParseEntry(subReader));
                        reader.ReadEndElement();
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        private static Entry ParseEntry(XmlReader reader)
        {
            var fields = new Dictionary<string, string>();

            reader.ReadToFollowing("String");

            while (reader.Name == "String")
            {
                reader.Read();
                var name = reader.ReadElementContentAsString();
                var value = reader.ReadElementContentAsString();

                fields.Add(name, value);
                reader.ReadEndElement();
            }

            if (fields.Count == 0)
                return null;

            return new Entry(fields);
        }

        private static Group ParseGroup(XmlReader reader)
        {
            var group = new Group();

            reader.ReadToDescendant("Name");
            group.Name = reader.ReadElementContentAsString();

            ParseChildren(reader, group);

            return group;
        }
    }
}