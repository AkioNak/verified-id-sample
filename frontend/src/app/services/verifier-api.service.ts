import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { PresentationReqeustResult } from '../models/presentationRequestResult';
import { PresentationResponse } from '../models/presentationResponse';

@Injectable({
  providedIn: 'root',
})
export class VerifierApiService {
  constructor(private http: HttpClient) {}

  public presentationRequest(): Observable<PresentationReqeustResult> {
    return this.http.get<PresentationReqeustResult>(
      environment.apiEndPoint + 'api/verifier/presentation-request'
    );
  }

  public presentationResponse(id: string): Observable<PresentationResponse> {
    return this.http.get<PresentationResponse>(
      environment.apiEndPoint + 'api/verifier/presentation-response?id=' + id
    );
  }
}
