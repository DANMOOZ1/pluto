using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StandardAttackRule", menuName = "AttackRuleSO/StandardAttackRule")]
public class StandardAttackRule : AttackRule
{
    public override bool AttackRuleFunc(List<Node> path)
    {
        if (path.Count > 1) return false;
        return true;
    }
}

