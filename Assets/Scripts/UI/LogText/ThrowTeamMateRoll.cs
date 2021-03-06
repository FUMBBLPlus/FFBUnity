﻿using Fumbbl.Model;
using Fumbbl.Model.RollModifier;
using Fumbbl.Model.Types;
using System.Collections.Generic;
using System.Linq;

namespace Fumbbl.UI.LogText
{
    public class ThrowTeamMateRoll : LogTextGenerator<Ffb.Dto.Reports.ThrowTeamMateRoll>
    {
        public override IEnumerable<LogRecord> Convert(Ffb.Dto.Reports.ThrowTeamMateRoll report)
        {
            Player thrower = FFB.Instance.Model.GetPlayer(report.playerId);
            Player thrownPlayer = FFB.Instance.Model.GetPlayer(report.thrownPlayerId);
            string neededRoll = "";

            // Make the passingDistance the first modifier and the reported ones follow it.
            IEnumerable<PassModifier> rollModifiers1 = report.passingDistance.As<PassModifier>().Yield();
            IEnumerable<PassModifier> rollModifiers2 = report.rollModifiers?.Select(r => r.As<PassModifier>());
            IEnumerable<PassModifier> rollModifiers = rollModifiers1.Concat(rollModifiers2);

            if (!report.reRolled)
            {
                yield return new LogRecord($"<b>{thrower.FormattedName} tries to throw {thrownPlayer.FormattedName}:</b>");

                if (rollModifiers.Contains(PassModifier.NervesOfSteel))
                {
                    yield return new LogRecord($"{thrower.FormattedName} is using Nerves of Steel to throw {thrower.Gender.Genetive} team-mate.");
                }
            }

            yield return new LogRecord($"<b>Throw Team-Mate Roll [ {report.roll} ]</b>", 1);

            if (report.successful)
            {
                yield return new LogRecord($"{thrower.FormattedName} throws {thrower.Gender.Genetive} team-mate successfully.", 2);

                if (!report.reRolled)
                {
                    neededRoll = $"Succeeded on a roll of {report.minimumRoll}+ to avoid a fumble";
                }
            }
            else
            {
                yield return new LogRecord($"{thrower.FormattedName} fumbles the throw.", 2);
                if (!report.reRolled)
                {
                    neededRoll = $"Roll a {report.minimumRoll}+ to avoid a fumble";
                }
            }

            if (!string.IsNullOrEmpty(neededRoll))
            {
                neededRoll += " (Roll ";

                if (rollModifiers != null)
                {
                    neededRoll += string.Join("", rollModifiers.Select(m => m.ModifierString));
                }

                neededRoll += " > 1).";

                yield return new LogRecord(neededRoll, 2);
            }
        }
    }
}
