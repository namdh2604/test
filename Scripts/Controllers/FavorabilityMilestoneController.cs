using System;
using System.Collections.Generic;
using Voltage.Witches.Models.MissionRequirements;
using Voltage.Witches.Shop;
using Voltage.Witches.Controllers.Favorability;
using Voltage.Story.StoryDivisions;
using Voltage.Witches.Screens;
using Voltage.Witches.Metrics;
using Voltage.Common.Metrics;
using Voltage.Witches.Models;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.Controllers
{
    public class FavorabilityMilestoneController
    {
        private readonly IScreenFactory _screenFactory;
        private readonly Player _player;
        private readonly int _affinityPerStarstone;

        private Dictionary<string, int> _affinityStarstoneData;
        private int _totalStarstonesNeededForAffinity;
        private SceneHeader _header;
        private Action<int> _callback;

        public FavorabilityMilestoneController(IScreenFactory screenFactory, Player player, MasterConfiguration masterConfig)
        {
            _screenFactory = screenFactory;
            _player = player;
            _affinityPerStarstone = masterConfig.Game_Properties_Config.Affinity_Per_Premium;

            _affinityStarstoneData = null;
            _totalStarstonesNeededForAffinity = 0;
        }

        public void Display(IEnumerable<AffinityRequirement> reqs, SceneHeader header, Action<int> callback)
        {
            _header = header;
            _callback = callback;

            CalculateStarstoneData(reqs);

            List<CharFavorabilityData> data = new List<CharFavorabilityData>();

            foreach (var req in reqs)
            {
                data.Add(new CharFavorabilityData(req.CharName, req.GetCurrentAffinity(), req.Amount));
            }

            FavorabilityMissionDialogViewModel model = new FavorabilityMissionDialogViewModel(data, header.PolaroidPath, _totalStarstonesNeededForAffinity);

            var dialog = _screenFactory.GetDialog<iGUISmartPrefab_FavorabilityMilestoneDialog>();
            dialog.Init(model);
            dialog.Display(HandleResponse);
        }

        private void CalculateStarstoneData(IEnumerable<AffinityRequirement> reqs)
        {
            _affinityStarstoneData = new Dictionary<string, int>();
            _totalStarstonesNeededForAffinity = 0;

            foreach (var entry in reqs)
            {
                int amountLacking = entry.Amount - entry.GetCurrentAffinity();
                if (amountLacking > 0)
                {
                    int amount = (int)Math.Ceiling(amountLacking / (double)_affinityPerStarstone);
                    _affinityStarstoneData[entry.CharName] = amount;
                    _totalStarstonesNeededForAffinity += amount;
                }
            }
        }

        private void HandleResponse(int response)
        {
            switch ((MileStoneDialogResponse)response)
            {
                case MileStoneDialogResponse.BUY_POTION:
                    if (_player.CurrencyPremium >= _totalStarstonesNeededForAffinity)
                    {
                        // immediately award the player with the appropriate affinities, shoot them to the scene
                        foreach (var entry in _affinityStarstoneData)
                        {
                            string charId = entry.Key[0].ToString();
                            _player.AddAffinity(charId, entry.Value * _affinityPerStarstone);
                        }

                        _player.UpdatePremiumCurrency(-_totalStarstonesNeededForAffinity);

                        SendMetricForAffinityPurchase(_header);
                        _callback((int)MileStoneDialogResponse.RESUME);
                    }
                    else
                    {
                        _callback((int)MileStoneDialogResponse.BUY_POTION);
                    }
                    return;
            }

            _callback(response);
        }

        private void SendMetricForAffinityPurchase(SceneHeader header)
        {
            string eventname = MetricEvent.PURCHASE_AFFINITY;

            IDictionary<string, object> data = new Dictionary<string, object>
            {
                {"route_id", header.Route},
                {"scene_id", header.Scene},
            };

            foreach (var neededAffinity in _affinityStarstoneData)
            {
                data[neededAffinity.Key] = neededAffinity.Value;
            }

            data["requiredStones"] = _totalStarstonesNeededForAffinity;

            AmbientMetricManager.Current.LogEvent(eventname, data);
        }
    }
}

