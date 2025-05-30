import { Component, OnInit } from '@angular/core';
import { LoaderService } from '@services/loader.service';

@Component({
  selector: 'sest-layout-loader',
  templateUrl: './layout-loader.component.html',
  styleUrls: ['./layout-loader.component.scss']
})
export class LayoutLoaderComponent implements OnInit {
  
  loading: boolean;

  constructor(public loaderService: LoaderService) {
    this.loaderService.isLoading.subscribe((v) => {
        this.loading = v;
    });
  }

  ngOnInit(): void {
  }

}
