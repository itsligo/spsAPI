using Microsoft.AspNet.Identity;
using spsServerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Cors;
//using System.Web.Mvc;

namespace spsAPI.Controllers
{
    [AllowAnonymous]
    //[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
    //[Authorize(Roles = "admin")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Views")]
    public class ListViewController : ApiController
    {
        private Model db = new Model();

        [Route("GetPlacementTypes")]
        public IQueryable<PlacementTypeListView> GetProgrammeTypes()
        {
            return db.PlacementTypeListViews;
        }
        [Route("GetPlacements/{year:int}")]
        public IQueryable<PlacementView> GetPlacements(int? year)
        {
            if (year != null)
                return db.PlacementViews.Where(p => p.StartDate.Value.Year == year);
            else return null;
        }

        [Route("GetAvailablePlacements")]
        public IQueryable<AvailablePlacementsView> GetAvailablePlacements()
        {
            return db.AvailablePlacementsViews;
        }


        [Route("GetAvailablePlacements/{year:int}")]
        public IQueryable<AvailablePlacementsView> GetAvailablePlacements(int? year)
        {
            if (year != null)
                return db.AvailablePlacementsViews.Where(p => p.StartDate.Value.Year == year);
            else return null;
        }


        [Route("GetPlacements")]
        public IQueryable<PlacementView> GetPlacements()
        {
                return db.PlacementViews;
        }

        [Route("GetPlacementYears")]
        public IQueryable<int?> GetYearList()
        {
            var years = db.PlacementYearsViews.Select(p => p.PlacementYear).Distinct();
            return years;
        }


        [Route("GetProgrammeList")]
        public IQueryable<ProgrammeListView> GetProgrammeList()
        {
            return db.ProgrammeListViews;
        }

        
        [Route("GetProgrammeStages")]
        public IQueryable<ProgrammeStagesView> GetProgrammeStages()
        {
            return db.ProgrammeStagesViews;
        }

        [Route("GetProvidersNames")]
        public IQueryable<ProvidersNameListView> GetProviderNames()
        {
            return db.ProvidersNameListViews;
        }

        [Route("GetPlacementYearsJson")]
        public dynamic GetYearListJsoned()
        {
            //var years = db.PlacementYearsViews.Select(p => p.PlacementYear).Distinct();
            var yearList = (from p in db.PlacementYearsViews
                           where p.PlacementYear != null
                           select new
                           {
                               id = p.PlacementYear,
                               year = p.PlacementYear
                           });
            //var result = yearList.ToDictionary(d => d.id, d => d.year);
            //return Json(result, JsonRequestBehavior.AllowGet);

            return yearList;
        }


        [Route("GetPlacementProviders")]
        public IQueryable<PlacementProviderView> GetProviders()
        {
            return db.PlacementProviderViews;
        }


    }
}
