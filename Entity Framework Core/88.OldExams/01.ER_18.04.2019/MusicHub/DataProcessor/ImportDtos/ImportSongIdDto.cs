﻿using System.Xml.Serialization;

namespace MusicHub.DataProcessor.ImportDtos
{
    [XmlType("Song")]
    public class ImportSongIdDto
    {
        [XmlAttribute("id")]

        public int SongId { get; set; }
    }
}