## Описание
Утилита для просмотра результатов сборки и тестов Azure.

## Запуск
Для запуска необходимо добавить файл SpecialAppSettings.config с таким содержимым:
```
<?xml version="1.0" encoding="utf-8" ?>
<appSettings>
	<add key="AzureUrlBase" value="https://{azure-url}/{organization}/{project}/"/>
</appSettings>
```
где в параметры `{azure-url}`, `{organization}`, `{project}` надо вставить нужные значения.

## Полезные ссылки
### build
https://{azure-url}/{organization}/{project}/_apis/build/builds?buildId=1165935&$top=100&api-version=7.1

### runs
https://{azure-url}/{organization}/{project}/_apis/test/runs?buildIds={buildIds}&$top={$top}&api-version=7.1
https://{azure-url}/{organization}/{project}/_apis/test/runs?buildUri=vstfs:///Build/Build/1165935&$top=100&includeRunDetails=true&api-version=7.1

### result
https://{azure-url}/{organization}/{project}/_apis/test/Runs/{runId}/results?api-version=7.1
