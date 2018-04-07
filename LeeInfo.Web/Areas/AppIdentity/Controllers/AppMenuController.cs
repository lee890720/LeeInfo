using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LeeInfo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace LeeInfo.Web.Areas.AppIdentity.Controllers
{
    [Area("AppIdentity")]
    [Authorize(Roles = "Admins")]
    public class AppMenuController : Controller
    {
        private readonly AppDbContext _context;

        public AppMenuController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AppIdentity/AppSidemenu
        public async Task<IActionResult> Index()
        {
            return View(await _context.AppMenu.ToListAsync());
        }

        // GET: AppIdentity/AppSidemenu/Create
        public IActionResult Create()
        {
            return PartialView("~/Areas/AppIdentity/Views/AppMenu/CreateEdit.cshtml");
        }

        // POST: AppIdentity/AppSidemenu/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Grade,Sequence,Follow,Ico,Url,Area,Controller,Action,Valid,Description,State")] AppMenu appMenu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appMenu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView("~/Areas/AppIdentity/Views/AppMenu/CreateEdit.cshtml",appMenu);
        }

        // GET: AppIdentity/AppSidemenu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appMenu = await _context.AppMenu.SingleOrDefaultAsync(m => m.Id == id);
            if (appMenu == null)
            {
                return NotFound();
            }
            return PartialView("~/Areas/AppIdentity/Views/AppMenu/CreateEdit.cshtml",appMenu);
        }

        // POST: AppIdentity/AppSidemenu/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Grade,Sequence,Follow,Ico,Url,Area,Controller,Action,Valid,Description,State")] AppMenu appMenu)
        {
            if (id != appMenu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appMenu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppMenuExists(appMenu.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return PartialView("~/Areas/AppIdentity/Views/AppMenu/CreateEdit.cshtml",appMenu);
        }

        // GET: AppIdentity/AppSidemenu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appMenu = await _context.AppMenu
                .SingleOrDefaultAsync(m => m.Id == id);
            if (appMenu == null)
            {
                return NotFound();
            }

            return PartialView("~/Areas/AppIdentity/Views/AppMenu/Delete.cshtml",appMenu.Name);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id,IFormCollection form)
        {
            var appMenu = await _context.AppMenu.SingleOrDefaultAsync(m => m.Id == id);
            _context.AppMenu.Remove(appMenu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppMenuExists(int id)
        {
            return _context.AppMenu.Any(e => e.Id == id);
        }
    }
}
