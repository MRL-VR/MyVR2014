using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MRL.Exploration.Frontiers;
using MRL.Mapping;


namespace MRL.Exploration.MultiAgent
{
    public class MultiExp
    {
        private List<Frontier> frontierList = new List<Frontier>();
        private EGMap _gMap = null;
        private const float minPruningThreshold = 2.0f;

        public void Init(EGMap egMap)
        {
            _gMap = egMap;
        }

        public void UpdateFrontiers(List<Frontier> list)
        {
            List<Frontier> validatedFrontiers = validationFrontiers(list);
            List<Frontier> prunedFrontiers = pruningFrontiers(validatedFrontiers);
            frontierList.AddRange(prunedFrontiers);
        }

        private List<Frontier> validationFrontiers(List<Frontier> fList)
        {
            List<Frontier> selectedFrontiers = new List<Frontier>();
            foreach (var item in fList)
                if (!_gMap.isWall(item.FrontierPosition))
                    selectedFrontiers.Add(item);

            return selectedFrontiers;
        }

        private List<Frontier> pruningFrontiers(List<Frontier> fList)
        {
            List<Frontier> selectedFrontiers = new List<Frontier>();

            foreach (var sItem in fList)
            {
                List<Frontier> list = frontierList.Where(a =>
                                                            (Math.Sqrt(Math.Pow((a.FrontierPosition.X - sItem.FrontierPosition.X), 2) -
                                                             Math.Pow((a.FrontierPosition.Y - sItem.FrontierPosition.Y), 2))) < minPruningThreshold).ToList();

                if (list.Count < 1)
                    selectedFrontiers.Add(sItem);

            }

            return selectedFrontiers;
        }


    }
}
