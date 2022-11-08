# verified-id-sample
[Microsoft Entra Verified ID](https://learn.microsoft.com/ja-jp/azure/active-directory/verifiable-credentials/decentralized-identifier-overview)を利用したデジタル証明書の発行および検証を行うアプリケーションです。

### 前提
- Microsoft Entra Verified IDの環境構築は既に行われているものとします。
  - 環境構築手順については下記のURLをご参照ください。
    - https://learn.microsoft.com/ja-jp/azure/active-directory/verifiable-credentials/verifiable-credentials-configure-tenant
    - https://learn.microsoft.com/ja-jp/azure/active-directory/verifiable-credentials/verifiable-credentials-configure-issuer
    - https://learn.microsoft.com/ja-jp/azure/active-directory/verifiable-credentials/verifiable-credentials-configure-verifier

- アプリケーション実行環境は下記の通りとなります。
  - Backend
    - ライブラリ：ASP .NET
    - ランタイム：.NET 6
  - Frontend
    - フレームワーク：Angular v14
    - Node バージョン： Node v16.18.0

また、お手持ちのモバイル端末に Microsoft Authenticator がない場合は下記の URL からインストールします。
- https://www.microsoft.com/ja-jp/security/mobile-authenticator-app
- Microsoft Authenticator を初回起動した際に Microsoft アカウントへのサインインが聞かれますが、Verified ID では不要ですのでスキップして構いません。

### 起動手順
#### Backend
1. Backendはインターネットゾーン経由でアクセスできる必要がありますので、Azure App Servicesを作成しておきます。
    - 名前：任意の名前
    - ランタイムスタック：.NET6
    - オペレーティング システム：Windows
    - App Serviceプラン：無ければ新規作成

    他はデフォルトで問題ありません。

1. `backend/VerifiedIdApi/appsettings.json`を編集し、お使いのVerified IDの環境に書き換えます。
    - Endpoint：`https://verifiedid.did.msidentity.com/v1.0/`
    - VCServiceScope：`3db474b9-6a0c-4840-96ac-1fceb342124f/.default`
    - Instance: `https://login.microsoftonline.com/{0}`
    - TenantId: ADのテナントID
        - Microsoft Entra管理ページのAzure Active Directory > Overviewから取得できます
    - ClientId: Verified ID用ADアプリケーションのID
        - Microsoft Entra管理ページのAzure Active Directory > Applications > アプリの登録から取得できます
    - ClientSecret: Verified ID用ADアプリケーションのシークレットの値
        - Microsoft Entra管理ページのAzure Active Directory > Applications > アプリの登録 > Verified ID用アプリ > 証明書とシークレットから取得できます
        - ただし、シークレットの値に関してはシークレット登録直後しか確認ができません。
    - IssuerAuthority および VerifierAuthority: 分散化識別子 (DID)
        - Microsoft Entra管理ページの検証済みID > 組織の設定から取得できます
    - CredentialManifest: マニフェスト URL
    - Type: 資格情報の種類
        - Microsoft Entra管理ページの検証済みID > 資格情報 > 詳細から取得できます
    - APIKey: APIキー
        - Verified ID～Backend間の認証用APIキーです
        - こちらは任意の値を設定してください。

    ```json
    {
      "VerifiedId": {
        "Endpoint": "https://verifiedid.did.msidentity.com/v1.0/",
        "VCServiceScope": "3db474b9-6a0c-4840-96ac-1fceb342124f/.default",
        "Instance": "https://login.microsoftonline.com/{0}",
        "TenantId": "<Tenant ID>",
        "ClientId": "<AD Application Client ID>",
        "ClientSecret": "<AD Application Client Secret>",
        "IssuerAuthority": "<DID>",
        "VerifierAuthority": "<DID>",
        "CredentialManifest": "<Manifest URL>",
        "Type": "<Type>",
        "ApiKey": "<API Key>"
      }
    }
    ```
 1. `backend/VerifiedIdApi`プロジェクトを作成したAzure App Servicesにデプロイします。
 
 #### Frontend
 1. `frontend/src/environemnts/environment.ts`を編集
    - `apiEndPoint`を先ほどデプロイしたApp Servicesのドメインに書き換えます。ドメインの最後に`/`を忘れずに追加します。
    
    ```ts
    export const environment = {
      production: false,
      apiEndPoint: 'https://<API EndPoint>/'
    };
    ```
 1. Frontendの起動
    - `frontend`ディレクトリに移動し、下記のコマンドでライブラリインストールを行います。
      ```
      > npm ci
      ```
    - 下記のコマンドでアプリケーションを起動します。
      ```
      > ng serve --open
      ```
    - Frontendが起動したら、自動的に`http://localhost:4200/`にアクセスされます。
