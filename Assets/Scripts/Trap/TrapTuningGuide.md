# Countdown Trap 调节速查

测试白模位置：`DemoScene/Countdown Trap Test Arena`。
红色检测范围由 `TrapDetectionZoneGizmo` 绘制；在 Scene 视图右上角开启 Gizmos 即可查看。

## 通用

1. 每个 Trap 根物体包含机关与检测范围；Trap 3 的底板和四面墙都是其子物体。
2. 在 Inspector 选中 Trap 根物体后调节对应脚本的字段。
3. 所有 Trap 在一次 Play 中只触发一次；停止再开始 Play 即可重置。
4. 调整检测范围后，确认红框覆盖玩家正常会走到的位置，同时保留可躲避的边缘。
5. 机关 Collider 使用 `Trap` Layer，并应用到所有子物体；攀爬扫描不包含此 Layer，但物理碰撞仍有效。
6. Trigger Collider 只检测不阻挡；平台、Meme 与墙使用非 Trigger Collider 来承重、推人或挡路。
7. Player 的 `LocomotionController` 与 `FootStepEffects` Ground Layer 必须包含 `Default | Trap`，不要把 `Trap` 加入攀爬扫描。

## Trap 1 - Moving Platform Trap

根物体：`Trap 1 - Moving Platform Trap`（移动平台与检测范围在同一物体）。

1. `Move Offset`：平台触发后移动的世界坐标位移；改方向或距离。
2. `Move Duration`：完成移动的秒数；数值越小，平台移开得越突然。
3. `Detection Center`：检测框相对平台中心的偏移。
4. `Detection Size`：检测框长宽高；红框会实时同步。
5. `Trap 1 - Moving Platform Start` 只是静态承重白模，不挂陷阱脚本。

## Trap 2 - Meme Pusher Trap

根物体：`Trap 2 - Meme Pusher Trap`（检测范围与大 Meme 在同一 Trap 中）。

1. `Meme`：大 Meme 根物体引用；替换模型后，将新模型根物体拖入此字段。
2. 旋转 Meme 的 Transform 即可改变推进方向；它始终沿自身 `Forward` 移动。
3. `Meme Move Speed`：大 Meme 的移动速度。
4. `Meme Travel Distance`：大 Meme 的总移动距离。
5. 新 Meme 需带一个或多个非 Trigger Collider，接触到角色时才会将角色推走。
6. 编辑器中 Meme 默认显示；进入 Play 后隐藏，角色进入检测范围时出现。
7. `Trap 2 - Static Platform` 是场景静态平台，不属于 Trap 2 根物体。

## Trap 3 - Directional Wall Trap

根物体：`Trap 3 - Directional Wall Trap`（检测范围、四面墙和底板都在同一 Trap 中）。

1. 根物体的 `Box Collider` 就是检测范围；用其 `Center` 与 `Size` 调整红框。
2. `Forward/Right/Back/Left Wall` 分别引用四面墙；墙体为根物体的子物体。
3. `Floor Platform` 引用 `Trap 3 - Drop Floor`；底板必须是根物体的子物体且使用非 Trigger Collider。
4. 角色在红框内输入某方向时，对应方向的墙立即出现；四面墙都出现后，底板会消失。
5. 角色掉落并离开红框后，等待 `Reset Delay` 再恢复底板、隐藏四面墙并允许再次触发；当前为 `2` 秒。
6. `Input Threshold`：输入灵敏度，越低越容易触发；测试场景当前为 `0.05`。
7. `Direction Dot Threshold`：方向判定精度，越高越要求输入方向准确；当前为 `0.7`。
8. 四个 `Movement Direction`：设置各墙对应的世界方向；当前前/右/后/左为 `+X/-Z/-X/+Z`。
9. 调整墙体位置和缩放时，让它们贴住红框四周，避免覆盖检测范围内部。
10. 编辑器中四面墙和底板默认显示；进入 Play 后四面墙隐藏，底板保留。

## 测试

进入 Play 后按正常路线走入红框，确认陷阱能触发且玩家可从检测范围边缘绕开。
