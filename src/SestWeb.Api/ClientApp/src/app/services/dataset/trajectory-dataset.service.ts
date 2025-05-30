import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { DatasetService } from '@services/dataset/dataset.service';

import { Trajetoria } from 'app/repositories/models/trajetoria';
import { TrajectoryUtils } from '@utils/dataset/trajectory';

@Injectable({
    providedIn: 'root'
})
export class TrajectoryDatasetService {

    public static readonly TRAJECTORY_KEY = 'TRAJECTORY_';

    $trajectoryChanged = new Subject<Trajetoria>();

    constructor(private dataset: DatasetService) { }

    add(trajectory: Trajetoria, caseId: string) {
        trajectory.canvas = TrajectoryUtils.loadTrajetoria(this.dataset.getById(caseId), trajectory) as any;
        this.dataset.add(TrajectoryDatasetService.getTrajectoryId(caseId), trajectory);
    }

    get(caseId: string): Trajetoria | undefined {
        return this.dataset.getById(TrajectoryDatasetService.getTrajectoryId(caseId));
    }

    update(trajectory: Trajetoria) {
        this.dataset.update(TrajectoryDatasetService.getTrajectoryId(this.dataset.currCaseId), trajectory);
        this.$trajectoryChanged.next(trajectory);
    }

    public static getTrajectoryId(caseId: string) {
        return TrajectoryDatasetService.TRAJECTORY_KEY + caseId;
    }
}
