## [ 建立階段 ]
# 使用 .NET SDK 映像作為基礎映像
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# 設定工作目錄
WORKDIR /src
# 把本機的.csproj 檔案複製到工作目錄
COPY works/*.csproj ./works/
# 根據.csproj列出的NuGet 套件下載/還原套件
RUN dotnet restore "works/works.csproj"

# 整個專案資料夾的內容複製進來
COPY . .
# 用release模式建置dll並發佈到/app/publish
RUN dotnet publish "works/works.csproj" -c Release -o /app/publish

## [ 執行階段 ]
# 使用只包含 .NET Runtime 的更小image，不含 SDK
FROM mcr.microsoft.com/dotnet/aspnet:8.0
# 設定工作目錄
WORKDIR /app
# 從建立階段複製已發佈的檔案到執行階段image
COPY --from=build /app/publish .

# 設定container啟動時執行的程式，會執行dotnet works.dll，啟動ASP.NET Core應用程式
ENTRYPOINT ["dotnet", "works.dll"]