using gunluk.Dtos;
using gunluk.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace gunluk.Controllers
{

    [Authorize]//token'sız işlem yapılmaması için
    public class NotlarController : ApiController
    {
        ApplicationDbContext db;
        public NotlarController()
        {
            db = new ApplicationDbContext();
        }
        public IEnumerable<NotBaslikDto> GetBasliklar()
        {
            string loggedInUserID = User.Identity.GetUserId();
            return db.Notlar.Where(n => n.ApplicationUserID == loggedInUserID).Select(n => new NotBaslikDto { ID = n.ID, Baslik = n.Baslik });

        }

        public IHttpActionResult GetNot(int id)
        {
            string loggedInUserID = User.Identity.GetUserId();

            Not n = db.Notlar.FirstOrDefault(k => k.ApplicationUserID == loggedInUserID && k.ID == id);

            if (n == null)
                return NotFound();

            return Ok(n);
        }

        [HttpPost]
        public HttpResponseMessage Save(NotKaydetDto not)
        {
            string loggedInUserID = User.Identity.GetUserId();
            NotKaydedildiDto mesaj = new NotKaydedildiDto();

            // yeni kayıt ekleme...
            if (not.ID == 0)
            {
                Not n = new Not();

                n.ApplicationUserID = loggedInUserID;
                n.EklenmeTarihi = DateTime.Now;
                n.Baslik = not.Baslik;
                n.Icerik = not.Icerik;

                db.Notlar.Add(n);
                db.SaveChanges();

                mesaj.ID = n.ID;
                mesaj.Mesaj = "Eklendi (" + n.EklenmeTarihi.Value.ToString() + ")";
            }
            // mevcut kaydın güncellenmesi...
            else
            {
                Not n = db.Notlar.FirstOrDefault(k => k.ApplicationUserID == loggedInUserID && k.ID == not.ID);

                if (n == null)
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bir hata oluştu!..");

                n.Baslik = not.Baslik;
                n.Icerik = not.Icerik;
                n.SonGuncellenmeTarihi = DateTime.Now;

                db.SaveChanges();

                mesaj.ID = n.ID;
                mesaj.Mesaj = "Güncellendi (" + n.SonGuncellenmeTarihi.Value.ToString() + ")";
            }

            return Request.CreateResponse(HttpStatusCode.OK, mesaj);
        }

        [HttpPost]
        public HttpResponseMessage Delete(int id)
        {
            string loggedInUserID = User.Identity.GetUserId();

            Not n = db.Notlar.FirstOrDefault(k => k.ApplicationUserID == loggedInUserID && k.ID == id);

            if (n == null)
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bir hata oluştu!");

            string baslik = n.Baslik;

            db.Notlar.Remove(n);
            db.SaveChanges();

            string mesaj = baslik + " başlıklı not silindi.";

            NotSilDto cevap = new NotSilDto() { ID = id, Mesaj = mesaj };

            return Request.CreateResponse(HttpStatusCode.OK, cevap);

        }
    }
    }
