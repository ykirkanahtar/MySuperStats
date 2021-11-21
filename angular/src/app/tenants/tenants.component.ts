import { Component, Injector } from "@angular/core";
import { finalize } from "rxjs/operators";
import { BsModalService, BsModalRef } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import {
  TenantServiceProxy,
  TenantDto,
  TenantDtoPagedResultDto,
} from "@shared/service-proxies/service-proxies";
import { CreateTenantDialogComponent } from "./create-tenant/create-tenant-dialog.component";
import { EditTenantDialogComponent } from "./edit-tenant/edit-tenant-dialog.component";

class PagedTenantsRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

@Component({
  templateUrl: "./tenants.component.html",
  animations: [appModuleAnimation()],
})
export class TenantsComponent extends PagedListingComponentBase<TenantDto> {
  tenants: TenantDto[] = [];
  keyword = "";
  isActive: boolean | null;
  advancedFiltersVisible = false;
  sortingColumn: string;

  constructor(
    injector: Injector,
    private _tenantService: TenantServiceProxy,
    private _modalService: BsModalService
  ) {
    super(injector);
  }

  list(
    request: PagedTenantsRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    request.isActive = this.isActive;

    this._tenantService
      .getAllForSessionUser(
        request.keyword,
        this.sortingColumn,
        request.skipCount,
        request.maxResultCount
      )
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: TenantDtoPagedResultDto) => {
        this.tenants = result.items;
        console.log(this.tenants);
        this.showPaging(result, pageNumber);
      });
  }

  delete(tenant: TenantDto): void {
    abp.message.confirm(
      this.l("TenantDeleteWarningMessage", tenant.name),
      undefined,
      (result: boolean) => {
        if (result) {
          this._tenantService
            .delete(tenant.id)
            .pipe(
              finalize(() => {
                abp.notify.success(this.l("SuccessfullyDeleted"));
                this.refresh();
              })
            )
            .subscribe(() => {});
        }
      }
    );
  }

  createTenant(): void {
    this.showCreateOrEditTenantDialog();
  }

  editTenant(tenant: TenantDto): void {
    this.showCreateOrEditTenantDialog(tenant.id);
  }

  deleteTenant(tenant: TenantDto): void {
    abp.message.confirm(
      this.l("TenantDeleteWarning", tenant.name),
      undefined,
      (result: boolean) => {
        if (result) {
          //Todo : delete tenant service
        }
      }
    );
  }

  leaveTenant(tenant: TenantDto): void {
    abp.message.confirm(
      this.l("TenantRemoveWarning", tenant.name),
      undefined,
      (result: boolean) => {
        if (result) {
          this._tenantService
            .leaveTenant(tenant.id)
            .pipe(
              finalize(() => {
                window.location.reload();
              })
            )
            .subscribe(() => {});
        }
      }
    );
  }

  loginTenant(tenant: TenantDto): void {
    this._tenantService
      .loginToTenant(tenant.id)
      .pipe(
        finalize(() => {
          window.location.reload();
        })
      )
      .subscribe(() => {});
  }

  showCreateOrEditTenantDialog(id?: number): void {
    let createOrEditTenantDialog: BsModalRef;
    if (!id) {
      createOrEditTenantDialog = this._modalService.show(
        CreateTenantDialogComponent,
        {
          class: "modal-lg",
        }
      );
    } else {
      createOrEditTenantDialog = this._modalService.show(
        EditTenantDialogComponent,
        {
          class: "modal-lg",
          initialState: {
            id: id,
          },
        }
      );
    }

    createOrEditTenantDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }

  clearFilters(): void {
    this.keyword = "";
    this.isActive = undefined;
    this.getDataPage(1);
  }
}
