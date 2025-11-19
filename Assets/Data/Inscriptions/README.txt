铭契数据文件夹
==================

此文件夹用于存放铭契（Inscription）的ScriptableObject资产。

## 创建新铭契

1. 在Unity编辑器中，右键点击此文件夹
2. 选择 Create > TerracottaARPG > Inscription
3. 填写铭契的属性：
   - ID: 唯一标识符
   - Display Name: 显示名称
   - Description: 描述
   - Icon: 图标
   - Rarity: 稀有度（Common/Rare/Epic/Legendary）
   - Effect: 效果类型
   - Value: 效果数值
   - Duration: 持续时间（如适用）
   - Max Stacks: 最大叠加层数（如适用）

## 示例铭契

根据设计文档，以下是一些铭契示例：

### 饕餮·贪噬（Epic）
- 效果：处决后3秒内普攻附当前生命上限1%的震裂伤害（可叠3层）
- Effect Kind: OnExecute_ShatterDamage
- Value: 0.01
- Duration: 3.0
- Max Stacks: 3

### 云雷·坚壁（Rare）
- 效果：盾墙期间每次受击+1裂纹层（+3%伤害，-2%减伤，持续6s，可叠5）
- Effect Kind: Shield_OnHit_Stack
- Value: 0.03 (伤害增幅)
- Duration: 6.0
- Max Stacks: 5

### 玄武·定山（Epic）
- 效果：千钧坠扩大半径20%，对被击退目标必定暴击
- Effect Kind: Slam_GuaranteedCrit
- Value: 0.2 (半径增加)

### 魑魅·散毒（Rare）
- 效果：对"汞蚀"中的敌人+25%伤害；你对水银DoT免疫50%
- Effect Kind: Mercury_DamageBonus
- Value: 0.25
