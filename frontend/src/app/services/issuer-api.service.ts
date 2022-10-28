import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Claim } from '../models/claim';
import { IssuanceRequestResult } from '../models/issuanceRequestResult';
import { IssuanceResponse } from '../models/issuanceResponce';

@Injectable({
  providedIn: 'root',
})
export class IssuerApiService {
  constructor(private http: HttpClient) {}

  public issuanceRequest(request: Claim): Observable<IssuanceRequestResult> {
    return this.http.post<IssuanceRequestResult>(
      environment.apiEndPoint + 'api/issuer/issuance-request',
      request
    );
  }

  public issuanceResponse(id: string): Observable<IssuanceResponse> {
    return this.http.get<IssuanceResponse>(
      environment.apiEndPoint + 'api/issuer/issuance-response?id=' + id
    );
  }
}
