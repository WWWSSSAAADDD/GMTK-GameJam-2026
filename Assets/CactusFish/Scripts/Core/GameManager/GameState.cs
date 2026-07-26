public enum GameState
{
    None,       // 未初始化
    Launch,     // 启动中（加载配置、预热资源）
    MainMenu,   // 主菜单
    Playing,    // 游戏中
    Paused,     // 暂停
    GameOver    // 游戏结束
}