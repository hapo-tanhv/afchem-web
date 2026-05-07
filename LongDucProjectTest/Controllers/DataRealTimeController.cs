using LongDucProjectTest.Service;
using LongDucProjectTest.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LongDucProjectTest.Controllers
{
    public class DataRealTimeController : Controller
    {
        // GET: DataRealTime
        [HttpPost]
        public JsonResult Read(string[] names)
        {
            try
            {
                var result = RealtimeService.Instance.Read(names);
                return new JsonResult()
                {
                    Data = new { Status = true, Result = result },
                    ContentType = "application/json",
                    ContentEncoding = Encoding.UTF8,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = int.MaxValue
                };
            }
            catch (Exception ex)
            {
                return Json(new { Status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Write(WriteParam[] writeParams)
        {
            try
            {
                var result = RealtimeService.Instance.Write(writeParams);
                return new JsonResult()
                {
                    Data = new { Status = true, Result = result },
                    ContentType = "application/json",
                    ContentEncoding = Encoding.UTF8,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = int.MaxValue
                };
            }
            catch
            {
                return Json(new { Status = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}