# 快速开始

## 下载LipUI（仅Windows）

[点击我](https://ghproxy.com/https://github.com/NovelCraft/LipUI/releases/latest/download/LipUI-net462-win10-x64.zip)下载LipUI。下载后运行即可。首次运行时需要下载Lip。如果杀毒软件报毒，请将LipUI和Lip添加到信任列表中。

## 下载Lip

如果你使用的不是Windows，或希望使用命令行界面，可以前往<https://github.com/LipPkg/Lip/releases/latest>下载Lip。下载后放在方便的地方即可。

## 运行服务端

如果你使用LipUI，打开LipUI，安装NovelCraft Server。

如果你使用Lip，首先将环境变量`LIP_REGISTRY`设为`https://registry.novelcraft.games`，然后运行`lip install server`。

运行`Server.exe`（Windows）或`Server`（非Windows）。

你可以修改`config.json`和`whitelist.json`来配置服务端和白名单。请记住，你的智能体或客户端的令牌必须在白名单中才能连接到服务端。

如果你想要使用自己的地图，请将地图文件放在`worlds`文件夹中。

如果你希望结束服务端，请输入`stop`。

## 运行客户端

如果你使用LipUI，打开LipUI，安装NovelCraft Client。

如果你使用Lip，首先将环境变量`LIP_REGISTRY`设为`https://registry.novelcraft.games`，然后运行`lip install client`。

运行`Client.exe`（Windows）或`Client`（非Windows）。

你可以阅览客户端中的帮助界面来了解如何使用客户端。

## 回放

默认情况下，服务端结束后，将会将回放文件保存在`worlds`文件夹内存档文件夹中。

将整个存档文件夹放到客户端的`worlds`文件夹中，启动客户端，选择回放即可。

## 更多内容

请前往[NovelCraft文档](https://novelcraft.games)阅读。
