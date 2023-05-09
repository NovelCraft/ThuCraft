
# 评分标准

比赛中，选手的成绩将由评委根据选手的智能体的表现情况进行评分。评分共分为四个乘区，分别为行动、战斗与生存、探索、创造。总分为四个乘区分值之积，设总分为$Y$，各乘区分值分别为 $Y_\text{行动}$、 $Y_\text{战斗与生存}$、 $Y_\text{探索}$、 $Y_\text{创造}$，则总分为：

<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8">
  </head>
  <body>
    <p>$$Y = Y_\text{行动} \times Y_\text{战斗与生存} \times Y_\text{探索} \times Y_\text{创造} $$</p>
  </body>
</html>
以下分别做出说明：

## 行动乘区

行动乘区的输入变量包括：

- $X_\text{行动}$：智能体在游戏中进行的行动次数，包括移动、攻击、使用物品等行为。


则行动乘区的分值为：

$$
Y_\text{行动} = 10000 \times 
\
    \tanh{
        (X_\text{行动} / 10000)
    }
$$

## 战斗与生存乘区

战斗乘区的输入变量包括：

- $X_\text{杀敌}$：智能体在游戏中杀死的敌人数量。

- $X_\text{死亡}$：智能体在游戏中死亡的次数。

- $X_\text{造成伤害}$：智能体在游戏中造成的伤害数。

- $X_\text{受到伤害}$：智能体在游戏中受到的伤害数。

则战斗乘区的分值为：

<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8">
  </head>
  <body>
    <p>$$
\begin{aligned}
Y_\text{战斗与生存} = 
    &\frac{1}{1 + 20\%\times X_\text{死亡}} \times\\
    &(
        1 +
        5\% \times X_\text{杀敌} +\\
        &0.1\% \times X_\text{造成伤害} +\\
        &0.1\% \times X_\text{受到伤害}
    )
\end{aligned}
$$</p>
  </body>
</html>

## 探索乘区

探索乘区的输入变量包括：

- $X_\text{煤矿}$：挖掘的煤矿方块数量。

- $X_\text{铁矿}$：挖掘的铁矿方块数量。

- $X_\text{金矿}$：挖掘的金矿方块数量。

- $X_\text{钻石矿}$：挖掘的钻石矿方块数量。

- $X_\text{树叶}$：砍伐的树叶方块数量。

则探索乘区的分值为：

<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8">
  </head>
  <body>
    <p>$$
 \begin{aligned}
 Y_\text{探索} =
    1 +
    &1\% \times X_\text{煤矿} +\\
    &2\% \times X_\text{铁矿} +\\
    &4\% \times X_\text{金矿} +\\
    &8\% \times X_\text{钻石矿} +\\
    &1\% \times X_\text{树叶} 
 \end{aligned}
$$</p>
  </body>
</html>

## 创造乘区

创造乘区的输入变量包括：

- $X_\text{新的木制工具}$：智能体在游戏中首次获得某种木制工具的次数。例如，如果智能体在游戏中首次获得木斧，则 $X_\text{新的木制工具}$ 增加 1。同时，如果智能体在游戏中再次获得木斧，则 $X_\text{新的木制工具}$ 不再增加。但如果智能体在游戏中首次获得木镐，则 $X_\text{新的木制工具}$ 再次增加 1。

- $X_\text{新的石制工具}$：智能体在游戏中首次获得某种石制工具的次数。

- $X_\text{新的铁制工具}$：智能体在游戏中首次获得某种铁制工具的次数。

- $X_\text{新的金制工具}$：智能体在游戏中首次获得某种金制工具的次数。

- $X_\text{新的钻石制工具}$：智能体在游戏中首次获得某种钻石制工具的次数。

则创造乘区的分值为：

<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8">
  </head>
  <body>
    <p>$$
\begin{aligned}
Y_\text{创造} =
    1 +
    &1\% \times X_\text{新的木制工具} +\\
    &2\% \times X_\text{新的石制工具} +\\
    &4\% \times X_\text{新的铁制工具} +\\
    &8\% \times X_\text{新的金制工具} +\\
    &16\% \times X_\text{新的钻石制工具}
\end{aligned}
$$</p>
  </body>
</html>
