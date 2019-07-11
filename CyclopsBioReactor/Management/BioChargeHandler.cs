﻿namespace CyclopsBioReactor.Management
{
    using CommonCyclopsUpgrades;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    internal class BioChargeHandler : CyclopsCharger
    {
        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        private const float BatteryDrainRate = 1.90f;

        private BioAuxCyclopsManager manager;
        private BioAuxCyclopsManager Manager => manager ?? (manager = MCUServices.Find.AuxCyclopsManager<BioAuxCyclopsManager>(Cyclops));

        internal const int MaxBioReactors = 6;

        private bool producingPower = false;
        private float totalBioCharge = 0f;
        private float totalBioCapacity = 0f;

        private readonly Atlas.Sprite sprite;

        public override float TotalReserveEnergy
        {
            get
            {
                float totalPower = 0f;
                foreach (CyBioReactorMono reactor in this.Manager.CyBioReactors)
                    totalPower += reactor.Charge;

                return totalPower;
            }
        }

        public BioChargeHandler(TechType cyBioBooster, SubRoot cyclops) : base(cyclops)
        {
            sprite = SpriteManager.Get(cyBioBooster);
        }

        public override Atlas.Sprite StatusSprite()
        {
            return sprite;
        }

        public override string StatusText()
        {
            return NumberFormatter.FormatValue(totalBioCharge) + (producingPower ? "+" : string.Empty);
        }

        public override Color StatusTextColor()
        {
            return NumberFormatter.GetNumberColor(totalBioCharge, totalBioCapacity, 0f);
        }

        protected override float GenerateNewEnergy(float requestedPower)
        {
            float tempBioCharge = 0f;
            float tempBioCapacity = 0f;
            bool currentlyProducingPower = false;

            foreach (CyBioReactorMono reactor in this.Manager.CyBioReactors)
            {
                tempBioCharge += reactor.Charge;
                tempBioCapacity = reactor.Capacity;
                currentlyProducingPower |= reactor.ProducingPower;
            }

            producingPower = currentlyProducingPower;
            totalBioCharge = tempBioCharge;
            totalBioCapacity = tempBioCapacity;

            // No energy is created but we can check for updates in this method since it always runs
            return 0f;
        }

        protected override float DrainReserveEnergy(float requestedPower)
        {
            float charge = 0f;

            foreach (CyBioReactorMono reactor in this.Manager.CyBioReactors)
                charge += reactor.GetBatteryPower(BatteryDrainRate, requestedPower);

            return charge;
        }
    }
}
