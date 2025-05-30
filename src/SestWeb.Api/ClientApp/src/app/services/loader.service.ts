import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {

  public isLoading = new BehaviorSubject(false);
  private loading = 0;

  constructor() { }

  addLoading() {
    this.loading++;
    this.isLoading.next(this.loading > 0);
  }

  removeLoading() {
    this.loading--;
    this.isLoading.next(this.loading > 0);
  }
}
