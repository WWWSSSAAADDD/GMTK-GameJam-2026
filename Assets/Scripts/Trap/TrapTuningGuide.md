# Countdown Trap 调节速查

测试白模：`DemoScene/Countdown Trap Test Arena`。在 Scene 视图右上角开启 Gizmos，可看到红色检测范围和黄色效果范围。

## 通用
1. 每个 Trap 根物体包含触发区和机关内容；在根物体 Inspector 调节脚本字段。
2. `Detection Center` / `Detection Size` 决定红框；边缘应留出可绕开的空间。
3. `Trigger Once`：开启时本局只触发一次；关闭时角色离开红框后，机关等待 `Repeat Reset Delay` 恢复并可再次触发。Trap 3 用 `Reset Delay`，默认可重复。
4. 机关与 Trigger 使用 `Trap` Layer；Trigger 仅检测，平台、Meme、墙和方块使用非 Trigger Collider。
5. `Trap` 不会被攀爬扫描识别，但仍会物理碰撞；Player Ground Layer 保持 `Default | Trap`。

## Trap 1 - Moving Platform Trap
根物体：`Trap 1 - Moving Platform Trap`。
1. `Move Offset`：移动方向和距离；`Move Duration`：耗时，越小越突然。
2. `Detection Center` / `Detection Size`：检测框位置与大小。
3. `Trap 1 - Moving Platform Start` 是独立的静态承重白模。

## Trap 2 - Meme Pusher Trap
根物体：`Trap 2 - Meme Pusher Trap`。
1. `Meme`：大 Meme 根物体；替换模型后重新拖入，模型必须带非 Trigger Collider。
2. Meme 沿自身 `Forward` 移动；旋转 Transform 可改方向。
3. `Meme Move Speed` / `Meme Travel Distance`：速度和总移动距离。
4. 编辑器中 Meme 可见，Play 开始隐藏，进入红框后出现并接触推人。

## Trap 3 - Directional Wall Trap
根物体：`Trap 3 - Directional Wall Trap`。
1. 四个 Wall 引用对应四面墙；`Floor Platform` 引用根物体下的 `Drop Floor`。
2. 红框内向某方向输入，对应墙出现；四墙出现后底板消失。
3. 掉落并离开红框后，等待 `Reset Delay` 恢复；当前为 `2` 秒。
4. `Input Threshold` 越低越灵敏，`Direction Dot Threshold` 越高方向越严格；四个 `Movement Direction` 定义墙方向。
5. 编辑器中墙和底板可见；Play 开始仅隐藏墙，底板保留。

## Trap 4 - Disappearing Block Trap
根物体：`Trap 4 - Disappearing Block Trap`，子物体：`Trap 4 - Disappearing Block`。
1. `Disappearing Block`：要消失的方块或模型，可替换为任意带 Collider 的物体。
2. `Disappear Delay`：进入红框到消失的等待秒数；当前为 `0.15`。
3. 编辑器与 Play 开始时方块可见，触发后失活并让玩家掉落。

## Trap 5 - Appearing Obstacle Trap
根物体：`Trap 5 - Appearing Obstacle Trap`，子物体：`Trap 5 - Appearing Obstacle`；`Trap 5 - Static Walkway` 是独立承重白模。
1. `Appearing Obstacle`：要显形的墙或模型，须带非 Trigger Collider。
2. `Appear Delay`：进入红框到出现的等待秒数；`0` 为立刻出现。
3. 编辑器中障碍可见；Play 开始隐藏，进入红框后显形并阻挡前进。

## Trap 6 - Delayed Impulse Trap
根物体：`Trap 6 - Delayed Impulse Trap`；红框检测、黄框生效，黄色箭头为顶飞方向。
1. `Activation Delay`：进入红框后等待秒数；延迟结束时玩家须位于黄框内才生效。
2. `Effect Center` / `Effect Size`：黄色效果区位置和大小。
3. `Impulse Direction` 是本地方向；旋转 Trap 可整体改方向。`Impulse Speed` / `Impulse Duration` 决定顶飞力度和持续时间。
## 测试：进入 Play 后按正常路线走进红框，确认机关会触发，同时可从检测范围边缘绕开。
