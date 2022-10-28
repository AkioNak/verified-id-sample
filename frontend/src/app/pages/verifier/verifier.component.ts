import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { PresentationReqeustResult } from 'src/app/models/presentationRequestResult';
import { PresentationResponse } from 'src/app/models/presentationResponse';
import { VerifierApiService } from 'src/app/services/verifier-api.service';

@Component({
  selector: 'app-verifier',
  templateUrl: './verifier.component.html',
  styleUrls: ['./verifier.component.scss'],
})
export class VerifierComponent implements OnInit {
  public state = 0; // 証明書検証のステータス
  public url = ''; // QRを発行するURL
  public firstName = ''; // 姓
  public lastName = ''; // 名

  constructor(private verifierApiService: VerifierApiService) {}

  ngOnInit(): void {
    this.verifyCredential();
  }

  /**
   * 証明書検証を行う
   */
  public verifyCredential(): void {
    this.state++;
    this.verifierApiService.presentationRequest().subscribe((model) => {
      // 証明書検証
      this.state++;
      this.url = model.url;

      // 証明書検証のステータスを取得する（ポーリング）
      var interval = setInterval(() => {
        this.verifierApiService
          .presentationResponse(model.id)
          .subscribe((response) => {
            if (response.status === 'request_retrieved') {
              // QRの読み取りがされた
              if (this.state === 2) {
                this.state++;
              }
            }
            if (response.status === 'presentation_verified') {
              // 証明書の検証が完了
              this.state++;

              this.lastName = response.lastName;
              this.firstName = response.firstName;

              // ポーリングを辞める
              clearInterval(interval);
            }
          });
      }, 1000);
    });
  }
}
