using System;
using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;

namespace BggDataRetriever
{
    public class BaseExport
    {
        [JsonPropertyName("bgg_id")]
        [Name("id")]
        public int BggId { get; set; }

        [JsonPropertyName("name")]
        [Name("name")]
        public string Name { get; set; }

        [JsonPropertyName("yearpublished")]
        [Name("yearpublished")]
        public string YearPublished { get; set; }

        [JsonPropertyName("rank")]
        [Name("rank")]
        public int? Rank { get; set; }

        [JsonPropertyName("bayesaverage")]
        [Name("bayesaverage")]
        public double? BayesAverage { get; set; }

        [JsonPropertyName("average")]
        [Name("average")]
        public double? Average { get; set; }

        [JsonPropertyName("usersrated")]
        [Name("usersrated")]
        public int? UsersRated { get; set; }

        [JsonPropertyName("is_expansion")]
        [Name("is_expansion")]
        public bool? IsExpansion { get; set; }

        [JsonPropertyName("abstracts_rank")]
        [Name("abstracts_rank")]
        public int? AbstractsRank { get; set; }

        [JsonPropertyName("cgs_rank")]
        [Name("cgs_rank")]
        public int? CgsRank { get; set; }

        [JsonPropertyName("childrensgames_rank")]
        [Name("childrensgames_rank")]
        public int? ChildrensGamesRank { get; set; }

        [JsonPropertyName("familygames_rank")]
        [Name("familygames_rank")]
        public int? FamilyGamesRank { get; set; }

        [JsonPropertyName("partygames_rank")]
        [Name("partygames_rank")]
        public int? PartyGamesRank { get; set; }

        [JsonPropertyName("strategygames_rank")]
        [Name("strategygames_rank")]
        public int? StrategyGamesRank { get; set; }

        [JsonPropertyName("thematic_rank")]
        [Name("thematic_rank")]
        public int? ThematicRank { get; set; }

        [JsonPropertyName("wargames_rank")]
        [Name("wargames_rank")]
        public int? WarGamesRank { get; set; }
    }
    
    public class CollectionExport
    {
        // objectid
        [JsonPropertyName("bgg_id")]
        [Name("objectid")]
        public int BggId { get; set; }
        
        [JsonPropertyName("name")]
        [Name("objectname")]
        public required string Name { get; set; }
        
        [JsonPropertyName("yearpublished")]
        [Name("yearpublished")]
        public required string YearPublished { get; set; }
        
        // is_expansion str representation ["expansion", "standalone"]
        [JsonPropertyName("itemtype")]
        [Name("itemtype")]
        public required string ItemType { get; set; }
        
        [JsonPropertyName("averageweight")]
        [Name("avgweight")]
        public double AverageWeight { get; set; }
        
        [JsonPropertyName("numowned")]
        [Name("numowned")]
        public int? NumOwned { get; set; }
        
        [JsonPropertyName("minplayers")]
        [Name("minplayers")]
        public int MinPlayers { get; set; }
        
        [JsonPropertyName("maxplayers")]
        [Name("maxplayers")]
        public int MaxPlayers { get; set; }
        
        [JsonPropertyName("minplaytime")]
        [Name("minplaytime")]
        public int MinPlayTime { get; set; }
        
        [JsonPropertyName("maxplaytime")]
        [Name("maxplaytime")]
        public int MaxPlayTime { get; set; }
        
        [JsonPropertyName("playingtime")]
        [Name("playingtime")]
        public int PlayingTime { get; set; }
        
        [JsonPropertyName("bggrecplayers")]
        [Name("bggrecplayers")]
        public required string BggRecPlayers { get; set; }
        
        [JsonPropertyName("bggbestplayers")]
        [Name("bggbestplayers")]
        public required string BggBestPlayers { get; set; }
        
        [JsonPropertyName("bggrecagerange")]
        [Name("bggrecagerange")]
        public required string BggRecAgeRange { get; set; }
    }
}