﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurpleBuzz_Backend.DAL;
using PurpleBuzz_Backend.Models;
using PurpleBuzz_Backend.ViewModels.Contact;

namespace PurpleBuzz_Backend.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public ContactController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var contactIntro = await _appDbContext.ContactIntro.FirstOrDefaultAsync();
            var contactContextComponent = await _appDbContext.ContactContextComponent.FirstOrDefaultAsync();
            var contactCommunicationComponents = await _appDbContext.ContactCommunicationComponents.ToListAsync();
            var model = new ContactIndexViewModel
            {
                ContactIntro = contactIntro,
                ContactContextComponent=contactContextComponent,
                ContactCommunicationComponent=contactCommunicationComponents
            };
            return View(model);
        }

    }
}
