
import { PermissionCheckerService } from 'abp-ng2-module';

import { Injectable } from '@angular/core';

@Injectable()
export class PermissionHelper {

    constructor(
        private _permissionCheckerService: PermissionCheckerService) {
    }

    editTenant(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Tenants.Edit');
    }

    setPassiveTenant(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Tenants.SetPassive');
    }


    getTenants(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Tenants');
    }

    getUsers(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Users');
    }

    userActivation(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Users.Activation');
    }

    getRoles(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Roles');
    }

    createMatch(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Matches.Create');
    }

    editMatch(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Matches.Edit');
    }

    deleteMatch(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Matches.Delete');
    }

    createTeam(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Teams.Create');
    }

    editTeam(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Teams.Edit');
    }

    deleteTeam(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Teams.Delete');
    }

    createPlayer(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Players.Create');
    }

    editPlayer(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Players.Edit');
    }

    deletePlayer(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Players.Delete');
    }

    createStat(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Stats.Create');
    }

    editStat(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Stats.Edit');
    }

    deleteStat(): boolean {
        return this._permissionCheckerService.isGranted('Pages.Stats.Delete');
    }
}

