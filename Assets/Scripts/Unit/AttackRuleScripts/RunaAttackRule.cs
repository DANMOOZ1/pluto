using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunaAttackRule", menuName = "AttackRuleSO/RunaAttackRule")]
public class RunaAttackRule : AttackRule
{
    public override bool AttackRuleFunc(List<Node> path,  Vector3Int pos)
    {
        if (path.Count is > 2 or 0) return false;
        
        bool flag = true;
        foreach (Node v in path)
        {
            if (v.x != pos[0])
            {
                flag = false;
                break;
            }
        }

        if (!flag)
        {
            flag = true;
            foreach (Node v in path)
            {
                if (v.y != pos[1])
                {
                    flag = false;
                    break;
                }
            }
        }
        return flag;
    }
}
