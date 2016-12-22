﻿using RimWorld;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace ImTheWorkerNow
{
    public static class ITWN
    {
        public static void PostMenuOption(
            List<FloatMenuOption> list,
            Pawn pawn,
            Thing target,
            string title,
            Action action,
            MenuOptionPriority priority = MenuOptionPriority.Default,
            Action mouseoverGuiAction = null,
            Thing revalidateClickTarget = null,
            float extraPartWidth = 0,
            Func<Rect, bool> extraPartOnGUI = null,
            WorkTypeDef workType = null)
        {
            Action handler = action;
            if (action != null && !pawn.CanReserve(target, 1))
            {
                Pawn reserver = pawn.Map.reservationManager.FirstReserverOf(target, pawn.Faction, true);
                title = title + " (" + "ReservedBy".Translate(new object[] { reserver.LabelShort }) + ")";
                handler = delegate
                {
                    Pawn currentReserver = pawn.Map.reservationManager.FirstReserverOf(target, pawn.Faction, true);
                    if (currentReserver != null && currentReserver.jobs != null)
                    {
                        try
                        {
                            ITWN.HorrifyingGlobalFakeryToPreventReserve = target;
                            currentReserver.jobs.EndCurrentJob(JobCondition.InterruptForced);
                        }
                        finally
                        {
                            ITWN.HorrifyingGlobalFakeryToPreventReserve = null;
                        }
                    }
                    Pawn newReserver = pawn.Map.reservationManager.FirstReserverOf(target, pawn.Faction, true);
                    if (newReserver != null)
                    {
                        Log.Error(string.Format("Something went wrong, {0}/{1}/{2} is the job history, please let the developer of I'm The Worker Now know!", currentReserver, newReserver, pawn));
                    }
                    else
                    {
                        action();
                    }
                };
            }

            if (workType != null && pawn.workSettings.GetPriority(workType) == 0)
            {
                title += string.Format(" (not assigned to {0})", workType.gerundLabel);
            }

            if (!list.Any((FloatMenuOption op) => op.Label == title.TrimEnd(new char[0])))
            {
                list.Add(new FloatMenuOption(title, handler, priority, mouseoverGuiAction, revalidateClickTarget, extraPartWidth, extraPartOnGUI));
            }
        }

        public static Thing HorrifyingGlobalFakeryToAllowReserve = null;
        public static Thing HorrifyingGlobalFakeryToPreventReserve = null;
    }
}
