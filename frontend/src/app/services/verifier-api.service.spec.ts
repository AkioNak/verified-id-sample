import { TestBed } from '@angular/core/testing';

import { VerifierApiService } from './verifier-api.service';

describe('VerifierService', () => {
  let service: VerifierApiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(VerifierApiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
