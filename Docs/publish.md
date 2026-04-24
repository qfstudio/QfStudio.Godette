# Toolbox

---

## Nuget cheatsheet

``` shell 
# 添加 Nuget 源
dotnet nuget add source --name lautsky --username lightyears --password PASSWORD_HERE https://lautsky-gitea.qfstudio.net/api/packages/GodotLab/nuget/index.json

# 打包
dotnet pack

# 推送（会自动顺带推送符号包）
dotnet nuget push ".\Toolbox\bin\Release\QfStudio.Toolbox.1.0.0.nupkg" --source lautsky
```
