# PawnFlow — tải ZIP từ Code, giải nén rồi chạy

## Dành cho giảng viên

1. Trên GitHub chọn **Code → Download ZIP**.
2. **Extract All / Giải nén tất cả** vào thư mục có quyền ghi, ví dụ Documents.
3. Mở **Chay-ung-dung.cmd** ở thư mục ngoài cùng vừa giải nén.
4. Lần đầu chờ chương trình tự chuẩn bị, sau đó trình duyệt tự mở. Tạo admin (mật khẩu 10–128 ký tự), rồi đăng nhập.
5. Giữ cửa sổ chạy mở. Nhấn Ctrl+C để dừng.

**Windows 10/11 64-bit. Không cần cài .NET SDK, .NET Runtime hoặc SQL Server. Không cần Internet để chạy. Không cần dùng Releases.**

Giữ nguyên các thư mục/file sau khi giải nén, đặc biệt `Runtime-goi` và `Khoi-dong.ps1`. Phần .NET/SQLite đã được kèm sẵn; launcher tự ghép, kiểm tra SHA256 và chuẩn bị Application lần đầu. Những lần sau chạy trực tiếp phần đã chuẩn bị.

Nếu trình duyệt không tự mở, chép URL hiện sau Website ready vào trình duyệt. Không chạy file CMD từ bên trong ZIP và không giải nén vào thư mục chỉ đọc như Program Files.

## Tài khoản và dữ liệu

Tự tạo admin ở lần đầu; không có mật khẩu mặc định hoặc dữ liệu cá nhân trong bản giao. Dữ liệu SQLite ở `Application/App_Data/pawnflow.db`, vẫn còn khi mở lại cùng thư mục. Bản giải nén mới ở thư mục khác có dữ liệu riêng. Bản này không dùng hoặc tự chuyển dữ liệu SQL Server cũ.

Sao lưu: dừng chương trình rồi sao chép cả Application/App_Data. Không đưa database/tài khoản cá nhân lên GitHub.

## Mã nguồn và yêu cầu đề bài

`PawnShopManagement/` chứa source ASP.NET Core MVC, EF Core SQLite, Bootstrap/jQuery và `Database/setup.sql` (script tạo SQLite database trống). App tự tạo schema, không cần thao tác SQL bằng tay. `Verification/` chứa bộ kiểm thử bằng SQLite thật.

Các chức năng: Admin/Staff; CRUD khách hàng/hợp đồng; tìm kiếm, filtering, paging, AJAX; session/cookies; annotations; REST API; CSRF, băm mật khẩu và khóa đăng nhập; tiếng Việt/Anh; thời hạn cầm và cảnh báo hạn.

Đề web.docx yêu cầu ASP.NET MVC, source, hướng dẫn cài/cấu hình và script tạo database; không chỉ định SQL Server. Bản này giữ MVC và dùng SQLite để chạy trên máy khác mà không cần cài database server. Cập nhật báo cáo/slide cũ từ SQL Server sang SQLite cho thống nhất.

## Dành cho người phát triển

Chỉ người sửa/build source cần .NET SDK 10 và Internet để tải thư viện. Người chấm dùng CMD đã có phần chạy kèm sẵn, không cần SDK.

Chạy `Kiem-tra.cmd` để kiểm thử source. `Dong-goi-ban-chay.cmd` để publish lại. Nếu thay đổi source, cần đóng gói lại bản chạy tương ứng; runtime đi kèm snapshot hiện tại, không tự biên dịch source trên máy người chấm.

## Nộp bài

Repository này đã gồm toàn bộ source và phần chạy đi kèm. Người chấm dùng Code → Download ZIP → giải nén toàn bộ → mở Chay-ung-dung.cmd. Không cần dùng Releases.

Phần Runtime-goi được gửi bằng Git cùng source, không cần cài .NET SDK/Runtime hay SQL Server trên máy người chấm. Bộ công cụ bên ngoài gói nộp có 2-DAY-LEN-GITHUB.cmd để người nộp tải toàn bộ lên repository của mình, tránh giới hạn Upload files trên trình duyệt.
