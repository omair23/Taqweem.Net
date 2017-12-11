﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taqweem.Models;

namespace Taqweem.ViewModels
{
    public class cPerpetualTime
    {
        public cPerpetualTime(DateTime Dval, Masjid Masjid)
        {
            Date = Dval;

            try
            {
                double PI = 4 * Math.Atan(1);
                double Pi = 4 * Math.Atan(1);
                double FAJR_ANGLE = 18;
                double ISHA_ANGLE = 18;
                double zSehriEnds, zFajr, zSunrise, zIshraaq, zZawaal, zAsr1, zAsr2, zSunset, zIftaar, zIsha;

                if (Masjid.JuristMethod == JuristicMethod.UniversityOfIslamicSciencesKarachi) { FAJR_ANGLE = 18; ISHA_ANGLE = 18; }
                else if (Masjid.JuristMethod == JuristicMethod.MuslimWorldLeague) { FAJR_ANGLE = 18; ISHA_ANGLE = 17; }
                else if (Masjid.JuristMethod == JuristicMethod.IslamicSocietyOfNorthAmerica) { FAJR_ANGLE = 15; ISHA_ANGLE = 15; }
                else if (Masjid.JuristMethod == JuristicMethod.UmmAlQuraUniversityMakkah) { FAJR_ANGLE = 18.5; ISHA_ANGLE = 0; }
                else if (Masjid.JuristMethod == JuristicMethod.EgyptianGeneralAuthorityOfSurvey) { FAJR_ANGLE = 19.5; ISHA_ANGLE = 17.5; }
                else { FAJR_ANGLE = 18; ISHA_ANGLE = 18; }

                double dblLatitude, dblLongitude, dblHeight, dblTimeZone;
                double CALC_1, CALC_2, CALC_3, CALC_4, CALC_5, CALC_6, DECLINATION;

                int GLO_YEAR = Dval.Year;// DateTime.Now.Year;
                dblLatitude = Masjid.Latitude * (PI / 180);
                dblLongitude = Masjid.Longitude;
                dblTimeZone = Masjid.TimeZone * 15;
                dblHeight = Masjid.Height;

                if (dblLatitude < 0)
                {
                    dblLatitude = -1 * dblLatitude;
                }

                DateTime LOOP_DATE = Dval;// DateTime.Now;
                int LOOP_MONTH = LOOP_DATE.Month;
                int LOOP_DAY = LOOP_DATE.Day;
                int LOOP_YEAR = LOOP_DATE.Year;
                if (LOOP_MONTH > 2) { LOOP_MONTH -= 3; } else { LOOP_YEAR -= 1; LOOP_MONTH += 9; }
                CALC_1 = ((0 / 24) + LOOP_DAY + Math.Floor(30.6 * LOOP_MONTH + 0.5) + Math.Floor(365.25 * (LOOP_YEAR - 1976)) - 8707.5) / 36525;
                CALC_2 = 23.4393 - (0.013 * CALC_1);
                CALC_3 = 357.528 + (35999.05 * CALC_1);
                CALC_3 = CALC_3 - (360 * Math.Floor(CALC_3 / 360));
                CALC_4 = (1.915 * Math.Sin(CALC_3 * PI / 180)) + (0.02 * Math.Sin(2 * CALC_3 * PI / 180));
                CALC_5 = 280.46 + (36000.77 * CALC_1) + CALC_4;
                CALC_5 = CALC_5 - (360 * Math.Floor(CALC_5 / 360));
                CALC_6 = CALC_5 - (2.466 * Math.Sin(2 * CALC_5 * PI / 180)) + (0.053 * Math.Sin(4 * CALC_5 * PI / 180));
                DECLINATION = Math.Atan(Math.Tan(CALC_2 * PI / 180) * Math.Sin(CALC_6 * PI / 180));

                if (Masjid.Latitude < 0) { DECLINATION = DECLINATION * -1; }
                double EQUATIONTIME = ((CALC_5 - CALC_4 - CALC_6) / 15);
                zZawaal = ((12 + ((dblTimeZone - dblLongitude) / 15) - EQUATIONTIME));// +0.0083333333;

                //SUNRISE AND SUNSET CALCULATIONS
                double AZIMUTH = ((Math.Sin((-0.8333 * PI / 180) - (0.0347 * Math.Sqrt(Masjid.Height) * Pi / 180)) - (Math.Sin(DECLINATION) * Math.Sin(dblLatitude))) / (Math.Cos(DECLINATION) * Math.Cos(dblLatitude)));
                AZIMUTH = Math.Atan(-AZIMUTH / Math.Sqrt(-AZIMUTH * AZIMUTH + 1)) + (Pi / 2);
                AZIMUTH = (180 / (15 * Pi)) * AZIMUTH;
                zSunrise = zZawaal - AZIMUTH;
                zSunset = zZawaal + AZIMUTH;
                zIftaar = zSunset + 0.05;
                zIshraaq = zSunrise + 0.0833333;

                double AZIMUTH2 = (((-1) * Math.Sin(FAJR_ANGLE * (PI / 180))) - ((Math.Sin(dblLatitude) * Math.Sin(DECLINATION)))) / (Math.Cos(dblLatitude) * Math.Cos((DECLINATION)));
                AZIMUTH2 = (Math.Acos(AZIMUTH2) * (180 / PI)) / 15;
                zSehriEnds = zZawaal - AZIMUTH2 - 0.083333333333;
                zFajr = zZawaal - AZIMUTH2;

                if (Masjid.JuristMethod == JuristicMethod.UmmAlQuraUniversityMakkah) { zIsha = zSunset + 0.05 + 1.5; }
                else
                {
                    double AZIMUTH3 = (((-1) * Math.Sin(ISHA_ANGLE * (PI / 180))) - ((Math.Sin(dblLatitude) * (Math.Sin(DECLINATION))))) / (Math.Cos(dblLatitude) * Math.Cos(DECLINATION));
                    AZIMUTH3 = (Math.Acos(AZIMUTH3) * (180 / PI)) / 15;
                    zIsha = zZawaal + AZIMUTH3;
                }

                double CALC_ASAR1 = (Math.Atan(1 / (1 + Math.Tan((dblLatitude) - (DECLINATION))))) * (180 / PI);
                CALC_ASAR1 = (Math.Sin(CALC_ASAR1 * (PI / 180))) - (Math.Sin(dblLatitude) * Math.Sin(DECLINATION));
                CALC_ASAR1 = CALC_ASAR1 / (Math.Cos(dblLatitude) * Math.Cos(DECLINATION));
                CALC_ASAR1 = (Math.Acos(CALC_ASAR1) * (180 / PI)) / 15;
                zAsr1 = zZawaal + CALC_ASAR1;

                double CALC_ASAR2 = Math.Atan(1 / (2 + (Math.Tan(((dblLatitude) - (DECLINATION)))))) * (180 / PI);
                CALC_ASAR2 = (Math.Sin(CALC_ASAR2 * (PI / 180))) - (Math.Sin(dblLatitude) * Math.Sin(DECLINATION));
                CALC_ASAR2 = CALC_ASAR2 / (Math.Cos(dblLatitude) * Math.Cos(DECLINATION));
                CALC_ASAR2 = (Math.Acos(CALC_ASAR2) * (180 / PI)) / 15;
                zAsr2 = zZawaal + CALC_ASAR2;

                int Day = Dval.Day;// DateTime.Now.Day;
                int Month = Dval.Month;// DateTime.Now.Month;
                int Year = Dval.Year;// DateTime.Now.Year;

                int SehriEndsHour = Convert.ToInt32(Math.Floor(zSehriEnds));
                int SunriseHour = Convert.ToInt32(Math.Floor(zSunrise));
                int IshraaqHour = Convert.ToInt32(Math.Floor(zIshraaq));
                int ZawaalHour = Convert.ToInt32(Math.Floor(zZawaal));
                int Asr1Hour = Convert.ToInt32(Math.Floor(zAsr1));
                int Asr2Hour = Convert.ToInt32(Math.Floor(zAsr2));
                int SunsetHour = Convert.ToInt32(Math.Floor(zSunset));
                int IshaHour = Convert.ToInt32(Math.Floor(zIsha));

                int SehriEndsMinute = Convert.ToInt32((zSehriEnds - (int)(zSehriEnds)) * 60);
                int SunriseMinute = Convert.ToInt32((zSunrise - (int)(zSunrise)) * 60);
                int IshraaqMinute = Convert.ToInt32((zIshraaq - (int)(zIshraaq)) * 60);
                int ZawaalMinute = Convert.ToInt32((zZawaal - (int)(zZawaal)) * 60);
                int Asr1Minute = Convert.ToInt32((zAsr1 - (int)(zAsr1)) * 60);
                int Asr2Minute = Convert.ToInt32((zAsr2 - (int)(zAsr2)) * 60);
                int SunsetMinute = Convert.ToInt32((zSunset - (int)(zSunset)) * 60);
                int IshaMinute = Convert.ToInt32((zIsha - (int)(zIsha)) * 60);

                int FajrHour = Convert.ToInt32(Math.Floor(zFajr));
                int FajrMinute = Convert.ToInt32((zFajr - (int)(zFajr)) * 60);

                SehriEnds = new DateTime(Year, Month, Day, SehriEndsHour, SehriEndsMinute, 0);
                Fajr = new DateTime(Year, Month, Day, FajrHour, FajrMinute, 0);
                Sunrise = new DateTime(Year, Month, Day, SunriseHour, SunriseMinute, 0);
                Ishraaq = new DateTime(Year, Month, Day, IshraaqHour, IshraaqMinute, 0);
                Zawaal = new DateTime(Year, Month, Day, ZawaalHour, ZawaalMinute, 0);
                Dhuhr = Zawaal.AddMinutes(5);
                AsrShafi = new DateTime(Year, Month, Day, Asr1Hour, Asr1Minute, 0);
                AsrHanafi = new DateTime(Year, Month, Day, Asr2Hour, Asr2Minute, 0);
                Sunset = new DateTime(Year, Month, Day, SunsetHour, SunsetMinute, 0);
                Maghrib = Sunset.AddMinutes(3);
                Isha = new DateTime(Year, Month, Day, IshaHour, IshaMinute, 0);
            }
            catch (Exception ex)
            {
                
            }
        }

        public DateTime Date { get; set; }

        public DateTime SehriEnds { get; set; } = new DateTime(1, 1, 1, 3, 1, 0);

        public DateTime Fajr { get; set;}

        public DateTime Sunrise { get; set;}

        public DateTime Ishraaq { get; set;}

        public DateTime Zawaal { get; set;}

        public DateTime Dhuhr { get; set;}

        public DateTime AsrShafi { get; set; }

        public DateTime AsrHanafi { get; set; }

        public DateTime Sunset { get; set;}

        public DateTime Maghrib { get; set; }

        public DateTime Isha { get; set;}

        public String twoDigitsFormat(int num)
        {
            return (num < 10) ? "0" + num : num + "";
        }

        // convert float hours to 24h format
        public String floatToTime24(double time)
        {
            if (time < 0)
                return "N/A";
            time = this.FixHour(time + 0.5 / 60);  // add 0.5 minutes to round
            double hours = Math.Floor(time);
            double minutes = Math.Floor((time - hours) * 60);
            return this.twoDigitsFormat((int)hours) + ":" + this.twoDigitsFormat((int)minutes);
        }

        // convert float hours to 12h format
        public String floatToTime12(double time, bool noSuffix)
        {
            if (time < 0)
                return "N/A";
            time = this.FixHour(time + 0.5 / 60);  // add 0.5 minutes to round
            double hours = Math.Floor(time);
            double minutes = Math.Floor((time - hours) * 60);
            String suffix = hours >= 12 ? " pm" : " am";
            hours = (hours + 12 - 1) % 12 + 1;
            return ((int)hours) + ":" + this.twoDigitsFormat((int)minutes) + (noSuffix ? "" : suffix);
        }

        // range reduce hours to 0..23
        public double FixHour(double hour)
        {
            hour = hour - 24.0 * (Math.Floor(hour / 24.0));
            hour = hour < 0 ? hour + 24.0 : hour;
            return hour;
        }

        private int JulianDate(int d, int m, int y)
        {
            int mm, yy;
            int k1, k2, k3;
            int j;
            yy = y - (int)((12 - m) / 10);
            mm = m + 9;
            if (mm >= 12)
            {
                mm = mm - 12;
            }
            k1 = (int)(365.25 * (yy + 4712));
            k2 = (int)(30.6001 * mm + 0.5);
            k3 = (int)((int)((yy / 100) + 49) * 0.75) - 38;
            // 'j' for dates in Julian calendar:
            j = k1 + k2 + d + 59;
            if (j > 2299160)
            {
                // For Gregorian calendar:
                j = j - k3;  // 'j' is the Julian date at 12h UT (Universal Time)
            }
            return j;
        }
    }
}
