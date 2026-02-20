namespace WarSim.Data
{
    /// <summary>
    /// Georgia/Caucasus region scenario data based on DCS World map.
    /// Blue faction: Georgia, Red faction: Russia
    /// </summary>
    public static class CaucasusScenarioData
    {
        public static class Airbases
        {
            // Blue (Georgia)
            public static readonly Location Batumi = new("Batumi International", 41.6102, 41.5997);
            public static readonly Location Kobuleti = new("Kobuleti", 41.9281, 41.8575);
            public static readonly Location Kutaisi = new("Kutaisi Kopitnari", 42.1768, 42.4827);
            public static readonly Location SenakiKolkhi = new("Senaki-Kolkhi", 42.2395, 42.0456);
            public static readonly Location Tbilisi = new("Tbilisi Lochini", 41.6692, 44.9547);

            // Red (Russia)
            public static readonly Location Gudauta = new("Gudauta", 43.0970, 40.5706);
            public static readonly Location SochiBabusheri = new("Sochi-Adler", 43.4499, 39.9566);
            public static readonly Location Anapa = new("Anapa Vityazevo", 45.0021, 37.3473);
            public static readonly Location Gelendzhik = new("Gelendzhik", 44.5692, 38.0125);
            public static readonly Location Krasnodar = new("Krasnodar Pashkovsky", 45.0347, 39.1705);
            public static readonly Location Mineralnye = new("Mineralnye Vody", 44.2251, 43.0819);
            public static readonly Location Mozdok = new("Mozdok", 43.7889, 44.6061);
            public static readonly Location Beslan = new("Beslan", 43.2051, 44.6064);
        }

        public static class Cities
        {
            // Georgia
            public static readonly Location BatumiCity = new("Batumi City", 41.6168, 41.6367);
            public static readonly Location Poti = new("Poti", 42.1477, 41.6718);
            public static readonly Location Zugdidi = new("Zugdidi", 42.5088, 41.8708);
            public static readonly Location TbilisiCity = new("Tbilisi City", 41.7151, 44.8271);
            public static readonly Location Gori = new("Gori", 41.9842, 44.1089);

            // Abkhazia
            public static readonly Location Sukhumi = new("Sukhumi", 43.0015, 41.0192);

            // Russia
            public static readonly Location Sochi = new("Sochi", 43.5855, 39.7231);
            public static readonly Location Tuapse = new("Tuapse", 44.0969, 39.0768);
            public static readonly Location Novorossiysk = new("Novorossiysk", 44.7233, 37.7686);
            public static readonly Location Vladikavkaz = new("Vladikavkaz", 43.0368, 44.6680);
        }

        public static class NavalZones
        {
            // Georgian coast
            public static readonly Location BatumiPort = new("Batumi Port", 41.6425, 41.6340);
            public static readonly Location PotiPort = new("Poti Port", 42.1547, 41.6620);

            // Abkhazia coast
            public static readonly Location SukhumiPort = new("Sukhumi Port", 43.0036, 41.0145);

            // Russian coast
            public static readonly Location SochiPort = new("Sochi Port", 43.5897, 39.7202);
            public static readonly Location TuapsePort = new("Tuapse Port", 44.1042, 39.0619);
            public static readonly Location NovorossiyskPort = new("Novorossiysk Port", 44.7193, 37.7827);
            public static readonly Location AnapaWaters = new("Anapa Waters", 44.8950, 37.3200);
        }

        public static class StrategicPoints
        {
            public static readonly Location RokiTunnel = new("Roki Tunnel", 42.7856, 44.0356);
            public static readonly Location EnguriBridge = new("Enguri Bridge", 42.8137, 41.5944);
            public static readonly Location SupsaBridge = new("Supsa Bridge", 42.1194, 41.9386);
        }

        public record Location(string Name, double Latitude, double Longitude);
    }
}
