import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Claim } from 'src/app/models/claim';
import { IssuanceRequestResult } from 'src/app/models/issuanceRequestResult';
import { IssuanceResponse } from 'src/app/models/issuanceResponce';
import { IssuerApiService } from 'src/app/services/issuer-api.service';

@Component({
  selector: 'app-issuer',
  templateUrl: './issuer.component.html',
  styleUrls: ['./issuer.component.scss'],
})
export class IssuerComponent {
  public state = 0; // 証明書発行のステータス
  public url = ''; // QRを発行するURL
  public pin = ''; // 証明書発行のPIN
  public result = ''; // 証明書発行結果
  public request = new Claim(); // 証明書発行時のリクエスト

  constructor(private issuerApiService: IssuerApiService) {}

  public issueCredential(): void {
    this.state++;
    this.issuerApiService.issuanceRequest(this.request).subscribe((model) => {
      this.state++;
      this.url = model.url;
      this.pin = model.pin;

      var interval = setInterval(() => {
        this.issuerApiService
          .issuanceResponse(model.id)
          .subscribe((response) => {
            if (response.status === 'request_retrieved') {
              // QRの読み取りがされた
              if (this.state === 2) {
                this.state++;
              }
            }
            if (response.status === 'issuance_successful') {
              // 証明書の発行が完了
              this.state++;

              this.result = JSON.stringify(response);
              // ポーリングを辞める
              clearInterval(interval);
            }
          });
      }, 1000);
    });
  }
}
