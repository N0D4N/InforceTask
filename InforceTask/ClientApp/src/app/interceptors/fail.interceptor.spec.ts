import {TestBed} from '@angular/core/testing';

import {FailInterceptor} from './fail.interceptor';

describe('FailInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      FailInterceptor
    ]
  }));

  it('should be created', () => {
    const interceptor: FailInterceptor = TestBed.inject(FailInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
