import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NotybarService {
  private notybarSubject = new Subject<any>();
  public notybarState = this.notybarSubject.asObservable();

  constructor() {}

  show(message: string, type?: 'success' | 'danger' | 'warning') {
    this.notybarSubject.next({
      show: true,
      message,
      type,
    });
  }
}
