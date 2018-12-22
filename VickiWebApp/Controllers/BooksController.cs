using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VickiWebApp.Models;

namespace VickiWebApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookContext _context;

        private readonly IHttpClientFactory _clientFactory;

        

        public BooksController(BookContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,FirstName,LastName,Title,ISBN,CoverType,DeweyDecimal")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,FirstName,LastName,Title,ISBN,CoverType,DeweyDecimal")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
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
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }


        public async Task<IActionResult> GetDewey(Book book)
        {
            string stringResult = "empty";
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://classify.oclc.org");
                    var response = await client.GetAsync($"/classify2/Classify?isbn={book.ISBN}&summary=true");
                    response.EnsureSuccessStatusCode();

                    stringResult = await response.Content.ReadAsStringAsync();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(stringResult);

                    XmlNodeList parentNode1 = xmlDoc.GetElementsByTagName("authors");
                    foreach (XmlNode childrenNode in parentNode1)
                    {
                            string[] fullName = childrenNode.InnerText.Split(',');
                        book.FirstName = fullName[1];
                        book.LastName = fullName[0];
                    }

                    XmlNodeList parentNode2 = xmlDoc.GetElementsByTagName("work");
                    foreach (XmlNode childrenNode in parentNode2)
                    {
                            book.Title = childrenNode.Attributes["title"].Value;
                    }

                    XmlNodeList parentNode3 = xmlDoc.GetElementsByTagName("mostPopular");
                    foreach (XmlNode childrenNode in parentNode3)
                    {
                        if (childrenNode.Name == "mostPopular"){
                            book.DeweyDecimal = childrenNode.Attributes["sfa"].Value;
                            break;
                        }                        
                    }

                    XmlNodeList parentNode4 = xmlDoc.GetElementsByTagName("input");
                    foreach (XmlNode childrenNode in parentNode4)
                    {
                        book.ISBN = childrenNode.InnerText;
                    }
                }
                catch (HttpRequestException httpRequestException)
                {
                    return BadRequest($"Error getting weather from OpenWeather: {httpRequestException.Message}");
                }
            }   
            
            return View("Create", book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
