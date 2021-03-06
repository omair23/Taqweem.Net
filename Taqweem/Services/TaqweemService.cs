﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taqweem.Classes;
using Taqweem.Data;
using Taqweem.Models;

namespace Taqweem.Services
{
    public class TaqweemService
    {
        private readonly EFRepository Repository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaqweemService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            Repository = new EFRepository(_context);
            _userManager = userManager;
        }

        //Masjid

        public Masjid MasjidCreate(Masjid masjid)
        {
            return Repository.Add(masjid);
        }

        public IEnumerable<Masjid> MasjidGetAll()
        {
            return Repository.GetAll<Masjid>();
        }

        public async Task<IEnumerable<Masjid>> MasjidGetAllAsync()
        {
            var myTask = Task.Run(() => Repository.GetAllAsync<Masjid>());

            return await myTask.ConfigureAwait(false);
        }

        public async Task<List<Masjid>> MasjidGetByCoordinateRangeAsync(double Latitude, double Longitude)
        {
            double LatitudeLimitUpper = Latitude + 10;
            double LatitudeLimitLower = Latitude - 10;

            double LongitudeLimitUpper = Longitude + 10;
            double LongitudeLimitLower = Longitude - 10;

            var myTask = Task.Run(() => Repository
                       .Find<Masjid>(s =>
                       s.Latitude > LatitudeLimitLower
                       && s.Latitude < LatitudeLimitUpper
                       && s.Longitude > LongitudeLimitLower
                       && s.Longitude < LongitudeLimitUpper
                       )
                       .Include(s => s.SalaahTimes)
                       .ToList());

            return await myTask.ConfigureAwait(false);
        }

        public async Task<IEnumerable<Masjid>> MasjidGetAllSalaahTimesAsync()
        {
            var myTask = Task.Run(() => Repository
                        .GetAllAsync<Masjid>());

            return await myTask.ConfigureAwait(false);
        }

        public async Task<IEnumerable<Masjid>> MasjidGetByTermAsync(string Term)
        {
            var myTask = Task.Run(() => Repository.Find<Masjid>(s => s.Name.Contains(Term) |
                                                s.Town.Contains(Term) |
                                                s.Country.Contains(Term))
                                                .Take(25));

            return await myTask.ConfigureAwait(false);
        }

        public IEnumerable<Masjid> MasjidGetByTerm(string Term)
        {
            return Repository.Find<Masjid>(s => s.Name.Contains(Term) |
                                                s.Town.Contains(Term) |
                                                s.Country.Contains(Term))
                            .Take(25);
        }

        public Masjid MasjidGetById(string Id)
        {
            return Repository.GetByID<Masjid>(s => s.Id == Id);
        }

        public async Task<Masjid> MasjidGetByIdAsync(string Id)
        {
            var myTask = Task.Run(() => Repository.GetByIdAsync<Masjid>(s => s.Id == Id));

            return await myTask.ConfigureAwait(false);
        }

        public Masjid MasjidGetByIdIncluded(string Id)
        {
            return Repository
                    .Find<Masjid>(s => s.Id == Id)
                    .Include(s => s.SalaahTimes)
                    .Include(s => s.TimeZone)
                    .FirstOrDefault();
        }

        public async Task<Masjid> MasjidGetByIdIncludedAsync(string Id)
        {
            var myTask = Task.Run(() => Repository
                    .Find<Masjid>(s => s.Id == Id)
                    .Include(s => s.SalaahTimes)
                    .Include(s => s.TimeZone)
                    .FirstOrDefault());

            return await myTask.ConfigureAwait(false);
        }

        public Masjid MasjidGetByOldSiteId(int OldId)
        {
            return Repository
                    .Find<Masjid>(s => s.OldSiteId == OldId)
                    .FirstOrDefault();
        }

        public SalaahTime GetSalaahTime(Masjid masjid, DateTime Val)
        {
            if (masjid.SalaahTimesType == SalaahTimesType.ScheduleTime)
            {
                var FirstTry = masjid.SalaahTimes
                            .Where(s => s.DayNumber <= Val.DayOfYear
                                    && s.Type == SalaahTimesType.ScheduleTime)
                            .OrderByDescending(x => x.DayNumber)
                            .FirstOrDefault();

                if (FirstTry == null)
                    FirstTry = masjid.SalaahTimes
                            .Where(s => s.TimeDate.Year <= Val.Year
                                    && s.Type == SalaahTimesType.ScheduleTime)
                            .OrderBy(x => x.DayNumber)
                            .FirstOrDefault();

                return FirstTry;
            }
            else
            {
                return masjid.SalaahTimes
                            .Where(s => s.DayNumber == Val.DayOfYear
                                    && s.TimeDate.Year <= Val.Year
                                    && s.Type == SalaahTimesType.DailyTime)
                            .OrderByDescending(x => x.TimeDate.Year)
                            .OrderByDescending(x => x.DayNumber)
                            .FirstOrDefault();
            }
        }

        //Users
        public List<ApplicationUser> UsersGetByMasjidId(string Id)
        {
            return Repository
                    .Find<ApplicationUser>(s => s.MasjidId == Id)
                    .ToList();
        }

        //Notices
        public List<Notice> NoticesGetByMasjidIdUnhidden(string Id)
        {
            return Repository
                    .Find<Notice>(s => s.MasjidId == Id && !s.IsHidden)
                    .ToList();
        }

        public SalaahTime NextSalaahTime(Masjid masjid, DateTime Val)
        {
            SalaahTime Time;

            if (masjid.SalaahTimesType == SalaahTimesType.ScheduleTime)
            {
                Time = masjid.SalaahTimes
                            .Where(s => s.DayNumber > Val.DayOfYear
                                    && s.Type == SalaahTimesType.ScheduleTime)
                            .OrderBy(x => x.DayNumber)
                            .FirstOrDefault();

                if (Time == null)
                {
                    Time = masjid.SalaahTimes
                            .Where(s => s.Type == SalaahTimesType.ScheduleTime)
                            .OrderBy(x => x.DayNumber)
                            .FirstOrDefault();

                    if (Time != null)
                    {
                        Time.TimeDate = new DateTime(Val.Year + 1, 1, 1);
                        Time.TimeDate = Time.TimeDate.AddDays(Time.DayNumber - 1);
                    }
                }
            }
            else
            {
                Time = masjid.SalaahTimes
                            .Where(s => s.DayNumber > Val.DayOfYear
                                    && s.Type == SalaahTimesType.DailyTime
                                    && s.TimeDate.Year <= Val.Year
                                    && s.IsATimeChange == true)
                            .OrderByDescending(s => s.TimeDate.Year)
                            .OrderBy(x => x.DayNumber)
                            .FirstOrDefault();

                if (Time == null)
                {
                    Time = masjid.SalaahTimes
                            .Where(s => s.Type == SalaahTimesType.DailyTime
                                    && s.TimeDate.Year <= Val.Year + 1
                                    && s.IsATimeChange == true)
                            .OrderByDescending(s => s.TimeDate.Year)
                            .OrderBy(x => x.DayNumber)
                            .FirstOrDefault();
                }
            }

            return Time;
        }

        public void DBInit()
        {
            List<Models.TimeZone> TimeZones = Repository.GetAll<Models.TimeZone>().ToList();

            if (TimeZones.Count < 1)
            {
                foreach (var Zone in TimeZoneInfo.GetSystemTimeZones())
                {
                    Models.TimeZone T = new Models.TimeZone();
                    T.Id = Zone.Id;
                    T.DaylightName = Zone.DaylightName;
                    T.DisplayName = Zone.DisplayName;
                    T.StandardName = Zone.StandardName;
                    T.SupportsDaylightSavingTime = Zone.SupportsDaylightSavingTime;

                    T.DefaultUTCDifference = cCalculations.GetTimeZoneDifference(T.Id, DateTime.Now);

                    TimeZones.Add(T);
                }

                Repository.AddMultiple(TimeZones);
            }

            List<Masjid> AllMasjids = Repository.GetAll<Masjid>().ToList();

            //DB INIT
            if (AllMasjids.Count < 1)
            {
                Masjid s = new Masjid();
                s.Id = "5f3e7169-ab20-4b34-bb27-2e86eefee2c1";
                s.Name = "Masjid Muaadh bin Jabal - Crosby";
                s.Town = "Johannesburg";
                s.Country = "South Africa";
                s.Latitude = -26.195149;
                s.Longitude = 27.990238;
                s.OldSiteId = 1;
                s.LadiesFacility = true;
                s.JummahFacility = true;
                s.Address = "114 Jamestown Avenue Crosby Johannesburg 2092";
                s.Contact = "Ml R Joosub, Ml S Maanjra, Br Abdur Rasheed, Br Faizal Suffla, Br Basheer Seedat";
                s.TimeZoneId = "South Africa Standard Time";

                Repository.Add(s);
            }
            ////

            List<ApplicationUser> Users = Repository.GetAll<ApplicationUser>().ToList();

            if (Users.Count < 1)
            {
                var OmairEmail = "omair334@gmail.com";

                var user = new ApplicationUser
                {
                    UserName = OmairEmail,
                    Email = OmairEmail,
                    Id = "513f1fe1-8e01-4c62-b332-ee8a0f7e2c29",
                    FullName = "Omair Kazi",
                    EmailConfirmed = true,
                    ActiveStatus = UserStatus.Active,
                    IsSuperUser = true,
                    CreatedAt = new DateTime(2016, 1, 1),
                    MasjidId = "5f3e7169-ab20-4b34-bb27-2e86eefee2c1"
                };

                var Password = "Open@1";

                var result = _userManager.CreateAsync(user, Password).Result;
            }
        }
    }
}
