using Microsoft.AspNetCore.Mvc;
using ContactsManagementApi.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContactsManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly string jsonFilePath = "contacts.json";

        [HttpGet]
        public async Task<ActionResult<List<Contact>>> GetContacts()
        {
            var contacts = await GetContactsFromFile();
            return Ok(contacts);
        }

        [HttpPost]
        public async Task<ActionResult<Contact>> AddContact(Contact contact)
        {
            var contacts = await GetContactsFromFile();
            contact.Id = contacts.Count + 1; // Auto-incrementing ID
            contacts.Add(contact);
            await SaveContactsToFile(contacts);
            return CreatedAtAction(nameof(GetContacts), new { id = contact.Id }, contact);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, Contact contact)
        {
            var contacts = await GetContactsFromFile();
            var existingContact = contacts.Find(c => c.Id == id);
            if (existingContact == null) return NotFound();

            existingContact.FirstName = contact.FirstName;
            existingContact.LastName = contact.LastName;
            existingContact.Email = contact.Email;
            await SaveContactsToFile(contacts);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contacts = await GetContactsFromFile();
            var contact = contacts.Find(c => c.Id == id);
            if (contact == null) return NotFound();

            contacts.Remove(contact);
            await SaveContactsToFile(contacts);
            return NoContent();
        }

        private async Task<List<Contact>> GetContactsFromFile()
        {
            if (!System.IO.File.Exists(jsonFilePath))
            {
                return new List<Contact>();
            }

            var json = await System.IO.File.ReadAllTextAsync(jsonFilePath);
            return JsonSerializer.Deserialize<List<Contact>>(json) ?? new List<Contact>();
        }

        private async Task SaveContactsToFile(List<Contact> contacts)
        {
            var json = JsonSerializer.Serialize(contacts);
            await System.IO.File.WriteAllTextAsync(jsonFilePath, json);
        }
    }
}
