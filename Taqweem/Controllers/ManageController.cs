﻿using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Taqweem.Models;
using Taqweem.Models.ManageViewModels;
using Taqweem.Services;
using Taqweem.Data;
using Taqweem.ViewModels.ManageViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace Taqweem.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly ApplicationDbContext _context;
        private readonly EFRepository Repository;
        private readonly IMapper _mapper;

        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ManageController(
          ApplicationDbContext context,
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder,
          IMapper mapper)
        {
            _context = context;
            Repository = new EFRepository(_context);
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _mapper = mapper;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public string AddNotice(string Content)
        {
            try
            {
                Notice Notice = new Notice();

                ApplicationUser user = _userManager.GetUserAsync(User).Result;

                Notice.NoticeContent = Content;

                Notice.MasjidId = user.MasjidId;
                Notice.IsHidden = false;
                Notice.CreatedId = user.Id;

                Repository.Add(Notice);

                return "Successful";
            }
            catch (Exception ex)
            {
                return "Failed " + ex.Message;
            }
        }

        public string HideNotice(string Id)
        {
            try
            {
                Notice Notice = Repository.Find<Notice>(s => s.Id == Id).FirstOrDefault();
                Notice.IsHidden = true;

                Repository.Update(Notice);
                return "Successful";
            }
            catch (Exception ex)
            {
                return "Failed " + ex.Message;
            }
        }

        public string DeleteNotice(string Id)
        {
            try
            {
                Notice Notice = Repository.Find<Notice>(s => s.Id == Id).FirstOrDefault();
                Repository.Delete(Notice);
                return "Successful";
            }
            catch (Exception ex)
            {
                return "Failed " + ex.Message;
            }
        }

        public string DeleteSalaahTime(string Id)
        {
            try
            {
                SalaahTime time = Repository.Find<SalaahTime>(s => s.Id == Id).FirstOrDefault();
                Repository.Delete(time);
                return "Successful";
            }
            catch (Exception ex)
            {
                return "Failed " + ex.Message;
            }
        }

        [HttpPost]
        public string UploadRapidsoft(IFormFile file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<SalaahTime> Times = new List<SalaahTime>();

                    ApplicationUser user = _userManager.GetUserAsync(User).Result;

                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        var text = reader.ReadToEnd();
    
                        text = text.Replace("\"", "");

                        string[] lines = text.Split(
                                        new[] { Environment.NewLine },
                                        StringSplitOptions.None
                                    );

                        bool SequenceFound = false;
                        string MonthLinker = "";

                        foreach (var Line in lines)
                        {
                            if (Line == "" | Line == null)
                                break;

                            string[] z = Line.Split(
                                new[] { ',' },
                                StringSplitOptions.None
                            );

                            if (SequenceFound == true)
                            {
                                SalaahTime Time = new SalaahTime();
                                Time.Type = SalaahTimesType.DailyTime;
                                Time.MasjidId = user.MasjidId;

                                if (z[0] != "")
                                {
                                    MonthLinker = z[0];
                                }
                                string DT = z[1] + " " + MonthLinker;
                                Debug.WriteLine(DT);

                                DateTime myDate = DateTime.ParseExact(DT, "d MMMM yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);

                                Time.TimeDate = myDate;
                                Time.DayNumber = myDate.DayOfYear;

                                Time.FajrAdhaan = StringToDateTime(z[10]);
                                Time.FajrSalaah = StringToDateTime(z[11]);
                                Time.DhuhrAdhaan = StringToDateTime(z[12]);
                                Time.DhuhrSalaah = StringToDateTime(z[13]);
                                Time.AsrAdhaan = StringToDateTime(z[14]);
                                Time.AsrSalaah = StringToDateTime(z[15]);
                                Time.IshaAdhaan = StringToDateTime(z[18]);
                                Time.IshaSalaah = StringToDateTime(z[19]);

                                if (z[2] == "Friday")
                                {
                                    Time.JumuahAdhaan = StringToDateTime(z[12]);
                                    Time.JumuahSalaah = StringToDateTime(z[13]);
                                }

                                Times.Add(Time);
                            }

                            if (z[0] == "#####")
                            {
                                SequenceFound = true;
                            }
                        }

                        List<SalaahTime> OldTimes = Repository
                                                    .Find<SalaahTime>(s => s.MasjidId == "5f3e7169-ab20-4b34-bb27-2e86eefee2c1")//user.MasjidId)
                                                    .ToList();

                        Times = Times.OrderBy(s => s.DayNumber).ToList();

                        CheckDbTimeChange(Times);

                        Repository.AddMultiple(Times);

                        Repository.DeleteMultiple(OldTimes);
                    }
                    return "Success";
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                return "Fail" + ex.Message;
            }
        }

        public string DeleteAllTimes()
        {
            try
            {
                ApplicationUser user = _userManager.GetUserAsync(User).Result;

                List<SalaahTime> Times = Repository.Find<SalaahTime>(s => s.MasjidId == user.MasjidId).ToList();

                Repository.DeleteMultiple(Times);

                return "Successful";
            }
            catch (Exception ex)
            {
                return "Failed " + ex.Message;
            }
        }

        public List<SalaahTime> CheckDbTimeChange(List<SalaahTime> Times)
        {
            foreach(var Time in Times)
            {
                SalaahTime PrevDay = Times.Where(s => s.DayNumber == Time.DayNumber - 1).FirstOrDefault();

                if (PrevDay != null)
                {
                    if (PrevDay.FajrAdhaan.TimeOfDay != Time.FajrAdhaan.TimeOfDay |
                        PrevDay.FajrSalaah.TimeOfDay != Time.FajrSalaah.TimeOfDay |
                        PrevDay.AsrAdhaan.TimeOfDay != Time.AsrAdhaan.TimeOfDay |
                        PrevDay.AsrSalaah.TimeOfDay != Time.AsrSalaah.TimeOfDay |
                        PrevDay.IshaAdhaan.TimeOfDay != Time.IshaAdhaan.TimeOfDay |
                        PrevDay.IshaSalaah.TimeOfDay != Time.IshaSalaah.TimeOfDay)
                        Time.IsATimeChange = true;
                }
            }

            return Times;
        }

        public DateTime StringToDateTime(string Val)
        {
            try
            {
                var t = Val.Split(':');

                int hour = Convert.ToInt32(t[0]);

                int minute = Convert.ToInt32(t[1]);

                DateTime dVal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, 0);

                return dVal;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }

        [HttpGet]
        public IActionResult AddSalaahTime()
        {
            SalaahTimeViewModel Model = new SalaahTimeViewModel();

            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSalaahTime(SalaahTimeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _userManager.GetUserAsync(User).Result;

            SalaahTime Time = new SalaahTime();

            Time.MasjidId = user.MasjidId;
            Time.Type = SalaahTimesType.ScheduleTime;
            Time.DayNumber = model.EffectiveDate.DayOfYear;

            //_mapper.Map(model, Time);

            Time.FajrAdhaan = model.FajrAdhaan;
            Time.FajrSalaah = model.FajrSalaah;

            Time.DhuhrAdhaan = model.DhuhrAdhaan;
            Time.DhuhrSalaah = model.DhuhrSalaah;
            Time.SpecialDhuhrAdhaan = model.SpecialDhuhrAdhaan;
            Time.SpecialDhuhrSalaah = model.SpecialDhuhrSalaah;
            Time.JumuahAdhaan = model.JumuahAdhaan;
            Time.JumuahSalaah = model.JumuahSalaah;

            Time.AsrAdhaan = model.AsrAdhaan;
            Time.AsrSalaah = model.AsrSalaah;

            Time.IshaAdhaan = model.IshaAdhaan;
            Time.IshaSalaah = model.IshaSalaah;

            //Time.UID = Repository.GetAll<SalaahTime>().Max(s => s.UID) + 1;

            Repository.Add(Time);

            StatusMessage = "The Salaah Time has been created";
            return RedirectToAction("SalaahTimes", "Manage");
        }

        public IActionResult SalaahTimes()
        {
            ApplicationUser user = _userManager.GetUserAsync(User).Result;

            Masjid masjid = Repository.Find<Masjid>(s => s.Id == user.MasjidId).FirstOrDefault();

            SalaahTimesViewModel Model = new SalaahTimesViewModel();

            Model.Type = masjid.SalaahTimesType;

            Model.MasjidId = masjid.Id;

            Model.SalaahTimes = Repository.Find<SalaahTime>(s => s.MasjidId == masjid.Id
                                                            && s.Type == masjid.SalaahTimesType)
                                                            .ToList();

            return View(Model);
        }

        [HttpGet]
        public IActionResult SalaahTimesAdmin(string Id)
        {
            ApplicationUser user = _userManager.GetUserAsync(User).Result;

            if (user.IsSuperUser)
            {
                Masjid masjid = Repository.Find<Masjid>(s => s.Id == Id).FirstOrDefault();

                //TO DO REPLACE VIEW MODEL

                MasjidEditViewModel Model = new MasjidEditViewModel();

                _mapper.Map(masjid, Model);

                return View("Masjid", Model);
            }
            else
            {
                return RedirectToAction("Masjid", "Manage");
            }
        }

        public IActionResult Notices()
        {
            ApplicationUser user = _userManager.GetUserAsync(User).Result;

            Masjid masjid = Repository.Find<Masjid>(s => s.Id == user.MasjidId).FirstOrDefault();

            NoticesViewModel Model = new NoticesViewModel();

            Model.MasjidId = masjid.Id;

            Model.Notices = Repository
                            .Find<Notice>(s => s.MasjidId == masjid.Id)
                            .Include(s => s.Created)
                            .ToList();

            return View(Model);
        }

        [HttpGet]
        public IActionResult Masjid()
        {
            ApplicationUser user = _userManager.GetUserAsync(User).Result;

            Masjid masjid = Repository.Find<Masjid>(s => s.Id == user.MasjidId).FirstOrDefault();

            MasjidEditViewModel Model = new MasjidEditViewModel();

            _mapper.Map(masjid, Model);

            return View(Model);
        }

        [HttpGet]
        public IActionResult MasjidAdmin(string Id)
        {
            ApplicationUser user = _userManager.GetUserAsync(User).Result;

            if (user.IsSuperUser)
            {
                Masjid masjid = Repository.Find<Masjid>(s => s.Id == Id).FirstOrDefault();

                MasjidEditViewModel Model = new MasjidEditViewModel();

                _mapper.Map(masjid, Model);

                return View("Masjid", Model);
            }
            else
            {
                return RedirectToAction("Masjid", "Manage");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Masjid(MasjidEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _userManager.GetUserAsync(User).Result;

            if (user == null | (user.MasjidId != model.Id && !user.IsSuperUser))
            {
                throw new ApplicationException($"Unable to update the masjid");
            }

            Masjid masjid = Repository.Find<Masjid>(s => s.Id == model.Id).FirstOrDefault();

            masjid.LastUpdate = DateTime.UtcNow;

            _mapper.Map(model, masjid);

            Repository.Update(masjid);

            StatusMessage = "The Masjid has been updated";
            return RedirectToAction(nameof(Masjid));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage,
                ShowDetails = user.ShowDetails,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = user.Email;

            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            var ShowDetails = user.ShowDetails;

            if (model.ShowDetails != ShowDetails)
            {
                ApplicationUser dbUser = Repository.Find<ApplicationUser>(s => s.Id == user.Id).FirstOrDefault();
                dbUser.ShowDetails = model.ShowDetails;
                Repository.Update(dbUser);
            }

            var FullName = user.FullName;

            if (model.FullName != FullName)
            {
                ApplicationUser dbUser = Repository.Find<ApplicationUser>(s => s.Id == user.Id).FirstOrDefault();
                dbUser.FullName = model.FullName;
                Repository.Update(dbUser);
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = HttpUtility.UrlEncode(code);

            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been set.";

            return RedirectToAction(nameof(SetPassword));
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
            model.StatusMessage = StatusMessage;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "The external login was added.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "The external login was removed.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            return View(nameof(Disable2fa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            _logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var model = new EnableAuthenticatorViewModel
            {
                SharedKey = FormatKey(unformattedKey),
                AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("model.Code", "Verification code is invalid.");
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            _logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
            return RedirectToAction(nameof(GenerateRecoveryCodes));
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            var model = new GenerateRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

            _logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

            return View(model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenicatorUriFormat,
                _urlEncoder.Encode("Taqweem"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        #endregion
    }
}
