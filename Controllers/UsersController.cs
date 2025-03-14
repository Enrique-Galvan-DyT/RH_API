using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Management;
using System.Web.Security;
using RH_API.Models;

namespace RH_API.Controllers
{
    public class UsersController : ApiController
    {
        private db_model db = new db_model();

        // GET: api/Users
        public List<User> GetUsers()
        {
            return db.Users.ToList();
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [HttpPut]
        public IHttpActionResult PutUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!db.Users.Any(x => x.IdUser == user.IdUser))
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        [HttpPost]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Users.Add(user);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = user.IdUser }, user);
        }

        // DELETE: api/Users/5
        [HttpPost]
        [Route("api/Users/Delete")]
        public IHttpActionResult DeleteUser(User user)
        {
            User userDelete = db.Users.Find(user.IdUser);
            if (userDelete == null)
            {
                return NotFound();
            }

            db.Users.Remove(userDelete);
            db.SaveChanges();

            return Ok(userDelete);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        [Route("api/Users/OrderDesc")]
        public List<User> orderDesc()
        {
            return db.Users.OrderByDescending(x => x.creation_date).ToList();
        }

        [HttpGet]
        [Route("api/Users/Names")]
        public List<string> Names()
        {
            return db.Users.Select(x => x.Name).ToList();
        }

        [HttpGet]
        [Route("api/Users/Emails")]
        public string[] EmailList()
        {
            return db.Users.Select(x => x.Email).ToArray();
        }

        [HttpGet]
        [Route("api/Users/Availables")]
        public List<User> UsersAvailables()
        {
            return db.Users.Where(x => x.Status == true).ToList();
        }

        [HttpPost]
        [Route("api/Users/Login")]
        public bool Login(User user)
        {
            if(db.Users.Any(x => x.Email == user.Email && x.Password == user.Password))
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        [Route("api/Users/Register")]
        public User Register(User user)
        {
            if (db.Users.Any(x => x.Email == user.Email))
            {
                return user;
            }
            db.Users.Add(user);
            db.SaveChanges();
            return user;
        }

        [HttpPost]
        [Route("api/Users/Unable")]
        public User Unable(User user)
        {
            user = db.Users.Find(user.IdUser);
            user.Status = false;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges(); 

            return user;
        }

        [HttpPost]
        [Route("api/Users/UnableRoleAndUsers")]
        public List<User> UnableRoleAndUsers(Role role)
        {
            role = db.Roles.Where(x => x.Name == role.Name).FirstOrDefault();

            role.Status = false;
            db.Entry(role).State = EntityState.Modified;

            foreach (var user in db.Users.Where(x => x.Role.Name == role.Name).ToList())
            {
                user.Status = false;
                db.Entry(role).State = EntityState.Modified;
            }

            db.SaveChanges();
            return db.Users.Where(x => x.Role.Name == role.Name).ToList();
            //return db.Roles.Where(x => x.Name == role.Name).SelectMany(r => r.Users).Where(u => u.Status == true).ToList();
            //return db.Users.Where(x => x.Status == false && x.Role.Name == role.Name).ToList();
        }

        [HttpPost]
        [Route("api/Users/EnableRole")]
        public Role EnableRole(Role role)
        {
            role = db.Roles.Where(x => x.Name == role.Name).FirstOrDefault();

            role.Status = true;
            db.Entry(role).State = EntityState.Modified;

            db.SaveChanges();
            return role;
        }

        [HttpPost]
        [Route("api/Users/EnableUsersByIds")]
        public List<User> EnableUsersByIds(int[] ids)
        {
            foreach (var user in db.Users.Where(x => ids.Contains(x.IdUser)))
            {
                user.Status = !user.Status;
                db.Entry(user).State = EntityState.Modified;
            }

            db.SaveChanges();
            return db.Users.Where(x => ids.Contains(x.IdUser)).ToList();
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.IdUser == id) > 0;
        }
    }
}