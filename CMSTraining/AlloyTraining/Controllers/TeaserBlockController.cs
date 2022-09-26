using System;
using System.Web.Mvc;
using AlloyTraining.Models.Blocks;
using AlloyTraining.Models.ViewModels;
using EPiServer.Web.Mvc;

namespace AlloyTraining.Controllers
{
    public class TeaserBlockController : BlockController<TeaserBlock>
    {
        public override ActionResult Index(TeaserBlock currentBlock)
        {
            var viewmodel = new TeaserBlockViewModel
            {
                CurrentBlock = currentBlock,
                TodaysVisitorCount = new Random().Next(300, 900)
            };
            return PartialView(viewmodel);
        }
    }
}