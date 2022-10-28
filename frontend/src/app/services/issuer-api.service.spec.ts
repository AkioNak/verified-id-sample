import { TestBed } from '@angular/core/testing';

import { IssuerApiService } from './issuer-api.service';

describe('IssuerService', () => {
  let service: IssuerApiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(IssuerApiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
