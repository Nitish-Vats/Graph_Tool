using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using Assignment.Models;
using System.Web.UI.DataVisualization.Charting;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult UploadExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase excelFile)
        {
            if (excelFile == null || excelFile.ContentLength == 0)
            {
                ViewBag.Error = "excelFile, Please select an Excel file to upload.";
                ModelState.AddModelError("excelFile", "Please select an Excel file to upload.");
                return View();
            }

            if (!excelFile.FileName.EndsWith(".xlsx"))
            {
                ViewBag.Error = "excelFile, The uploaded file must be an Excel file (.xlsx).";
                ModelState.AddModelError("excelFile", "The uploaded file must be an Excel file (.xlsx).");
                return View();
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelData = new List<MyObject>();
            using (var excelPackage = new ExcelPackage(excelFile.InputStream))
            {
             
                var worksheet = excelPackage.Workbook.Worksheets["Attributes"];

                if (worksheet==null)
                {
                    ViewBag.Error = "This is not Desired Excel File, Please Upload Correct One";
                    return View();
                }
                var rowCount = worksheet.Dimension.Rows;
                for (var row = 2; row <= rowCount; row++)
                {
                    var myObject = new MyObject
                    {
                        AttributeID = worksheet.Cells[row, 1].Value.ToString(),
                        AttributeLabel = worksheet.Cells[row, 2].Value.ToString()
                       
                    };
                    excelData.Add(myObject);
                }
            }

            var excelscore = new List<AttributesScore>();
            using (var excelPackage = new ExcelPackage(excelFile.InputStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets["AttributeScores"];
                if (worksheet == null)
                {
                    ViewBag.Error = "This is not Desired Excel File, Please Upload Correct One";
                    return View();
                }
                var rowCount = worksheet.Dimension.Rows;
                for (var row = 2; row <= rowCount; row++)
                {
                    var attributesScore = new AttributesScore
                    {
                        AttributeID = worksheet.Cells[row, 1].Value.ToString(),
                        Score = worksheet.Cells[row, 2].Value.ToString()

                    };
                    excelscore.Add(attributesScore);
                }
            }
            TempData["MyList1"] = excelData;
            TempData["MyList2"] = excelscore;



            return RedirectToAction("ExcelData");
        }
        public ActionResult ExcelData()
        {
           List<final> model = new List<final>();
            List<MyObject> data = (List<MyObject>)TempData["MyList1"];
            List<AttributesScore> score = (List<AttributesScore>)TempData["MyList2"];
            var query = from attribute in data
                        join sco in score on attribute.AttributeID equals sco.AttributeID
                        orderby sco.Score descending
                        select new final{ AttributeLabel = attribute.AttributeLabel, Score = sco.Score};

            var topFive = query.OrderByDescending(s => s.Score).Distinct(new FinalEqualityComparer()).Take(5).ToList();
            var bottomFive = query.OrderByDescending(s => s.Score).Reverse().Distinct(new FinalEqualityComparer()).Take(5).ToList();

            var viewModel = new BarGraphViewModel
            {
                TopFive = topFive.ToDictionary(x => x.AttributeLabel, x => x.Score),
               //TopFive = topFive,
                BottomFive = bottomFive.ToDictionary(x => x.AttributeLabel, x => x.Score)
            };


            ViewBag.TopFive = topFive;
            ViewBag.BottomFive = bottomFive;
            model = query.ToList();
            return View(viewModel);
        }
        public class FinalEqualityComparer : IEqualityComparer<final>
        {
            public bool Equals(final x, final y)
            {
                return x.AttributeLabel == y.AttributeLabel;
            }

            public int GetHashCode(final obj)
            {
                return obj.AttributeLabel.GetHashCode();
            }
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}