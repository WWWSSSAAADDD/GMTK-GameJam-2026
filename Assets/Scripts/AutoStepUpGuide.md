# 自动上楼梯配置

## 1. 开启自动上阶

1. 选择 `Parkour Controller/Player`，打开 `ParkourController`。
2. 勾选 `Enable Auto Step Up`。
3. `Auto Step Forward Input Threshold` 默认使用 `0.5`：角色明显向前移动才触发；误触发时调高，侧移也要触发时调低。
4. `Parkour Actions` 列表中必须包含要使用的上阶动作，例如 `StepUp`、`MediumStepUp`。

## 2. 配置可自动触发的台阶动作

1. 打开 `Assets/Fantacode Studios/Parkour & Climbing System/Resources/Parkour Actions/` 中的动作资产。
2. 勾选该动作的 `Auto Trigger On Forward Input`。
3. 用 `Min Height`、`Max Height` 限制可跨越的单级高度。
4. 默认 `StepUp` 为 `0.2-0.3m`，`MediumStepUp` 为 `0.3-0.8m`；每级台阶建议高度至少 `0.35m`，以触发 `MediumStepUp`。
5. 台阶需要有非 Trigger 的 Collider，并放在 `EnvironmentScanner > Obstacle Layer` 包含的 Layer；当前 Player 使用 `Default`。

## 3. 小台阶

`CharacterController > Step Offset` 当前为 `0.30m`。低于或等于这个高度的小台阶会直接走上去，不播放 Parkour 动画。

## 4. 防止上阶后自动 Crouch

1. 选择 `Parkour Controller/Player`，打开 `LocomotionController`。
2. 保持 `Enable Balance Walk` 开启。
3. 将 `Balance Walk Detection Type` 设置为 `Tagged`，不要使用 `Dynamic`。
4. 普通楼梯和平台不需要 Tag；只有需要平衡行走的窄梁才标记 `NarrowBeam` 或 `SwingableLedge`。

## 5. 测试场地

DemoScene 中的 `Auto Step Up Test` 位于 `(4, 0, -32)`，从右向左前进即可测试三阶 `MediumStepUp`。
