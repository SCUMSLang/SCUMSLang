enum AIScript {
    JunkYardDog,
    MoveDarkTemplarsToRegion,
    ValueThisAreaHigher,
    EnterClosestBunker,
    SetGenericCommandTarget,
    MakeTheseUnitsPatrol,
    EnterTransport,
    ExitTransport,
    AINukeHere,
    AIHarassHere,
    CastDisruptionWeb,
    CastRecall,
    SendAllUnitsOnStrategicSuicideMissions,
    SendAllUnitsOnRandomSuicideMissions,
    TerranCustomLevel,
    ZergCustomLevel,
    ProtossCustomLevel,
    TerranExpansionCustomLevel,
    ZergExpansionCustomLevel,
    ProtossExpansionCustomLevel,
    TerranCampaignEasy,
    TerranCampaignMedium,
    TerranCampaignDifficult,
    TerranCampaignInsane,
    TerranCampaignAreaTown,
    ZergCampaignEasy,
    ZergCampaignMedium,
    ZergCampaignDifficult,
    ZergCampaignInsane,
    ZergCampaignAreaTown,
    ProtossCampaignEasy,
    ProtossCampaignMedium,
    ProtossCampaignDifficult,
    ProtossCampaignInsane,
    ProtossCampaignAreaTown,
    ExpansionTerranCampaignEasy,
    ExpansionTerranCampaignMedium,
    ExpansionTerranCampaignDifficult,
    ExpansionTerranCampaignInsane,
    ExpansionTerranCampaignAreaTown,
    ExpansionZergCampaignEasy,
    ExpansionZergCampaignMedium,
    ExpansionZergCampaignDifficult,
    ExpansionZergCampaignInsane,
    ExpansionZergCampaignAreaTown,
    ExpansionProtossCampaignEasy,
    ExpansionProtossCampaignMedium,
    ExpansionProtossCampaignDifficult,
    ExpansionProtossCampaignInsane,
    ExpansionProtossCampaignAreaTown,
    ClearPreviousCombatData,
    SetPlayerToEnemyHere,
    SetPlayerToAllyHere,
    SwitchComputerPlayerToRescuePassive,
    TurnOnSharedVisionOfPlayer1WithCurrentPlayer,
    TurnOnSharedVisionOfPlayer2WithCurrentPlayer,
    TurnOnSharedVisionOfPlayer3WithCurrentPlayer,
    TurnOnSharedVisionOfPlayer4WithCurrentPlayer,
    TurnOnSharedVisionOfPlayer5WithCurrentPlayer,
    TurnOnSharedVisionOfPlayer6WithCurrentPlayer,
    TurnOnSharedVisionOfPlayer7WithCurrentPlayer,
    TurnOnSharedVisionOfPlayer8WithCurrentPlayer,
    TurnOffSharedVisionOfPlayer1WithCurrentPlayer,
    TurnOffSharedVisionOfPlayer2WithCurrentPlayer,
    TurnOffSharedVisionOfPlayer3WithCurrentPlayer,
    TurnOffSharedVisionOfPlayer4WithCurrentPlayer,
    TurnOffSharedVisionOfPlayer5WithCurrentPlayer,
    TurnOffSharedVisionOfPlayer6WithCurrentPlayer,
    TurnOffSharedVisionOfPlayer7WithCurrentPlayer,
    TurnOffSharedVisionOfPlayer8WithCurrentPlayer,
    Terran3ZergTown,
    Terran5TerranMainTown,
    Terran5TerranHarvestTown,
    Terran6AirAttackZerg,
    Terran6GroundAttackZerg,
    Terran6ZergSupportTown,
    Terran7BottomZergTown,
    Terran7RightZergTown,
    Terran7MiddleZergTown,
    Terran8CondeferateTown,
    Terran9LightAttack,
    Terran9HeavyAttack,
    Terran10CondeferateTowns,
    Terran11ZergTown,
    Terran11LowerProtossTown,
    Terran11UpperProtossTown,
    Terran12NukeTown,
    Terran12PhoenixTown,
    Terran12TankTown,
    Terran1ElectronicDistribution,
    Terran2ElectronicDistribution,
    Terran3ElectronicDistribution,
    Terran1Shareware,
    Terran2Shareware,
    Terran3Shareware,
    Terran4Shareware,
    Terran5Shareware,
    Zerg1TerranTown,
    Zerg2ProtossTown,
    Zerg3TerranTown,
    Zerg4RightTerranTown,
    Zerg4LowerTerranTown,
    Zerg6ProtossTown,
    Zerg7AirTown,
    Zerg7GroundTown,
    Zerg7SupportTown,
    Zerg8ScoutTown,
    Zerg8TemplarTown,
    Zerg9TealProtoss,
    Zerg9LeftYellowProtoss,
    Zerg9RightYellowProtoss,
    Zerg9LeftOrangeProtoss,
    Zerg9RightOrangeProtoss,
    Zerg10LeftTealAttack,
    Zerg10RightTealSupport,
    Zerg10LeftYellowSupport,
    Zerg10RightYellowAttack,
    Zerg10RedProtoss,
    Protoss1ZergTown,
    Protoss2ZergTown,
    Protoss3AirZergTown,
    Protoss3GroundZergTown,
    Protoss4ZergTown,
    Protoss5ZergTownIsland,
    Protoss5ZergTownBase,
    Protoss7LeftProtossTown,
    Protoss7RightProtossTown,
    Protoss7ShrineProtoss,
    Protoss8LeftProtossTown,
    Protoss8RightProtossTown,
    Protoss8ProtossDefenders,
    Protoss9GroundZerg,
    Protoss9AirZerg,
    Protoss9SpellZerg,
    Protoss10MiniTowns,
    Protoss10MiniTownMaster,
    Protoss10OvermindDefenders,
    BroodWarProtoss1TownA,
    BroodWarProtoss1TownB,
    BroodWarProtoss1TownC,
    BroodWarProtoss1TownD,
    BroodWarProtoss1TownE,
    BroodWarProtoss1TownF,
    BroodWarProtoss2TownA,
    BroodWarProtoss2TownB,
    BroodWarProtoss2TownC,
    BroodWarProtoss2TownD,
    BroodWarProtoss2TownE,
    BroodWarProtoss2TownF,
    BroodWarProtoss3TownA,
    BroodWarProtoss3TownB,
    BroodWarProtoss3TownC,
    BroodWarProtoss3TownD,
    BroodWarProtoss3TownE,
    BroodWarProtoss3TownF,
    BroodWarProtoss4TownA,
    BroodWarProtoss4TownB,
    BroodWarProtoss4TownC,
    BroodWarProtoss4TownD,
    BroodWarProtoss4TownE,
    BroodWarProtoss4TownF,
    BroodWarProtoss5TownA,
    BroodWarProtoss5TownB,
    BroodWarProtoss5TownC,
    BroodWarProtoss5TownD,
    BroodWarProtoss5TownE,
    BroodWarProtoss5TownF,
    BroodWarProtoss6TownA,
    BroodWarProtoss6TownB,
    BroodWarProtoss6TownC,
    BroodWarProtoss6TownD,
    BroodWarProtoss6TownE,
    BroodWarProtoss6TownF,
    BroodWarProtoss7TownA,
    BroodWarProtoss7TownB,
    BroodWarProtoss7TownC,
    BroodWarProtoss7TownD,
    BroodWarProtoss7TownE,
    BroodWarProtoss7TownF,
    BroodWarProtoss8TownA,
    BroodWarProtoss8TownB,
    BroodWarProtoss8TownC,
    BroodWarProtoss8TownD,
    BroodWarProtoss8TownE,
    BroodWarProtoss8TownF,
    BroodWarTerran1TownA,
    BroodWarTerran1TownB,
    BroodWarTerran1TownC,
    BroodWarTerran1TownD,
    BroodWarTerran1TownE,
    BroodWarTerran1TownF,
    BroodWarTerran2TownA,
    BroodWarTerran2TownB,
    BroodWarTerran2TownC,
    BroodWarTerran2TownD,
    BroodWarTerran2TownE,
    BroodWarTerran2TownF,
    BroodWarTerran3TownA,
    BroodWarTerran3TownB,
    BroodWarTerran3TownC,
    BroodWarTerran3TownD,
    BroodWarTerran3TownE,
    BroodWarTerran3TownF,
    BroodWarTerran4TownA,
    BroodWarTerran4TownB,
    BroodWarTerran4TownC,
    BroodWarTerran4TownD,
    BroodWarTerran4TownE,
    BroodWarTerran4TownF,
    BroodWarTerran5TownA,
    BroodWarTerran5TownB,
    BroodWarTerran5TownC,
    BroodWarTerran5TownD,
    BroodWarTerran5TownE,
    BroodWarTerran5TownF,
    BroodWarTerran6TownA,
    BroodWarTerran6TownB,
    BroodWarTerran6TownC,
    BroodWarTerran6TownD,
    BroodWarTerran6TownE,
    BroodWarTerran6TownF,
    BroodWarTerran7TownA,
    BroodWarTerran7TownB,
    BroodWarTerran7TownC,
    BroodWarTerran7TownD,
    BroodWarTerran7TownE,
    BroodWarTerran7TownF,
    BroodWarTerran8TownA,
    BroodWarTerran8TownB,
    BroodWarTerran8TownC,
    BroodWarTerran8TownD,
    BroodWarTerran8TownE,
    BroodWarTerran8TownF,
    BroodWarZerg1TownA,
    BroodWarZerg1TownB,
    BroodWarZerg1TownC,
    BroodWarZerg1TownD,
    BroodWarZerg1TownE,
    BroodWarZerg1TownF,
    BroodWarZerg2TownA,
    BroodWarZerg2TownB,
    BroodWarZerg2TownC,
    BroodWarZerg2TownD,
    BroodWarZerg2TownE,
    BroodWarZerg2TownF,
    BroodWarZerg3TownA,
    BroodWarZerg3TownB,
    BroodWarZerg3TownC,
    BroodWarZerg3TownD,
    BroodWarZerg3TownE,
    BroodWarZerg3TownF,
    BroodWarZerg4TownA,
    BroodWarZerg4TownB,
    BroodWarZerg4TownC,
    BroodWarZerg4TownD,
    BroodWarZerg4TownE,
    BroodWarZerg4TownF,
    BroodWarZerg5TownA,
    BroodWarZerg5TownB,
    BroodWarZerg5TownC,
    BroodWarZerg5TownD,
    BroodWarZerg5TownE,
    BroodWarZerg5TownF,
    BroodWarZerg6TownA,
    BroodWarZerg6TownB,
    BroodWarZerg6TownC,
    BroodWarZerg6TownD,
    BroodWarZerg6TownE,
    BroodWarZerg6TownF,
    BroodWarZerg7TownA,
    BroodWarZerg7TownB,
    BroodWarZerg7TownC,
    BroodWarZerg7TownD,
    BroodWarZerg7TownE,
    BroodWarZerg7TownF,
    BroodWarZerg8TownA,
    BroodWarZerg8TownB,
    BroodWarZerg8TownC,
    BroodWarZerg8TownD,
    BroodWarZerg8TownE,
    BroodWarZerg8TownF,
    BroodWarZerg9TownA,
    BroodWarZerg9TownB,
    BroodWarZerg9TownC,
    BroodWarZerg9TownD,
    BroodWarZerg9TownE,
    BroodWarZerg9TownF,
    BroodWarZerg10TownA,
    BroodWarZerg10TownB,
    BroodWarZerg10TownC,
    BroodWarZerg10TownD,
    BroodWarZerg10TownE,
    BroodWarZerg10TownF
}