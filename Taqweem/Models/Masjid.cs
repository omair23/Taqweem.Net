﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Taqweem.Classes;
using Taqweem.ViewModels;

namespace Taqweem.Models
{
    public enum JuristicMethod
    {
        UniversityOfIslamicSciencesKarachi = 1,
        MuslimWorldLeague = 2,
        IslamicSocietyOfNorthAmerica = 3,
        UmmAlQuraUniversityMakkah = 4,
        EgyptianGeneralAuthorityOfSurvey = 5
    }

    //    if (JMethod == 1) { FajrAngle = 18; IshaAngle = 18; txtJuristicM.Text = "University of Islamic Sciences, Karachi"; }
    //else if (JMethod == 2) { FajrAngle = 18; IshaAngle = 17; txtJuristicM.Text = "Muslim World League"; }
    //else if (JMethod == 3) { FajrAngle = 15; IshaAngle = 15; txtJuristicM.Text = "Islamic Society of North America"; }
    //else if (JMethod == 4) { FajrAngle = 18.5; IshaAngle = 0; txtJuristicM.Text = "Umm al-Qura University, Makkah"; }
    //else if (JMethod == 5) { FajrAngle = 19.5; IshaAngle = 17.5; txtJuristicM.Text = "Egyptian General Authority of Survey"; }
    //else { FajrAngle = 18; IshaAngle = 18; txtJuristicM.Text = "N/A"; }

    public class Masjid: AuditableEntity
    {
        public string Name { get; set; }

        public string Town { get; set; }

        public string Country { get; set; }

        public DateTime LastUpdate { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Height { get; set; }

        public double TimeZone { get; set; }

        public JuristicMethod JuristMethod { get; set; } = JuristicMethod.UniversityOfIslamicSciencesKarachi;

        public bool LadiesFacility { get; set; }

        public bool JummahFacility { get; set; }

        public string Contact { get; set; }

        public string Address { get; set; }

        public string GeneralInfo { get; set; }

        public bool AllowRegistration { get; set; } = true;

        public int MaghribAdhaanDelay { get; set; } = 3;

        [NotMapped]
        public virtual cSalaahTime SalaahTime {get; set;}

        [NotMapped]
        public double Distance { get; set; }

        public Masjid()
        {

        }

        public Masjid(string pId, double pLatitude, double pLongitude, double pHeight, double pTimezone)
        {
            Id = pId;
            Latitude = pLatitude;
            Longitude = pLongitude;
            Height = pHeight;
            TimeZone = pTimezone;

            LastUpdate = DateTime.UtcNow;
        }
    }
}
